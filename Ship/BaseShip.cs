using System.Collections.Generic;
using System.Linq;
using App.Physics;
using App.Ship.Generator;
using App.Ship.Gun;
using App.World;
using Godot;
using Godot.Collections;
using static Godot.GD;
using static Godot.Input;
using static Godot.Mathf;

namespace App.Ship;

public abstract partial class BaseShip : RigidBody3D
{
    public enum ShipType
    {
        Ship1,
        Ship2,
    }

    public enum EnergySlotType
    {
        Slot1,
        Slot2,
        Slot3,
    }
    
    public enum ShipComponent
    {
        Gun,
        Energy,
    }

    private const float SPEED = 5.0f;

    private Player Player => GetNode<Player>(SingletonPath.Player);
    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);
    private World.World World => GetParent<World.World>();

    private BaseShip _ship;
    private Camera.Camera Camera => GetNode<Camera.Camera>("Camera");
    private Node Slicer => GetNode<Node>("MeshSlicer");
    private Node3D Slicer1 => Slicer.GetNode<Node3D>("MeshSlicer1");
    private Node3D Slicer2 => Slicer.GetNode<Node3D>("MeshSlicer2");
    private AnimationTree Tree => GetNode<AnimationTree>("AnimationTree");
    private AudioStreamPlayer3D Engine01 => GetNode<AudioStreamPlayer3D>("Audio/Engine_01");
    private AudioStreamPlayer3D Explosion01 => GetNode<AudioStreamPlayer3D>("Audio/Explosion_01");

    protected Node GunSlots => GetNode("Component").GetNode("Gun");
    protected Node GeneratorSlots => GetNode("Component").GetNode("Generator");
    protected Material EnergyMaterial => GD.Load<Material>("res://Material/Laser.tres");
    protected MeshInstance3D Mesh => GetNode<MeshInstance3D>("MeshInstance3D");
    protected CollisionShape3D Shape => GetNode<CollisionShape3D>("CollisionShape3D");
    private Vector3 _direction;
    private Array<ArrayMesh> _meshes1;
    private Array<ArrayMesh> _meshes2;
    private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    private float _maxHeight = 6.0f;
    private float _minHeight = 1.2f;
    private float _maxXRotation = 30f;
    private float _maxZRotation = 30f;

    private float _maxSpeed;
    public float Speed = 1.5f;
    private float _maxShift = 20;
    protected abstract int GetControlResponsibilityPercent();

    private Pid _pid;
    private Vector3 _correctionImpulse;

    private int? _energyPercent = null;

    public int GetEnergyPercent()
    {
        if (_energyPercent == null)
        {
            _energyPercent = CalculateEnergyPercent();
        }

        return (int)_energyPercent;
    }

    public override void _Ready()
    {
        Player.MaxHeight = _maxHeight;
        Player.MinHeight = _minHeight;
        _prepare();
        ReCalculateEnergy();
        Engine01.Play();
    }

    public void Stop()
    {
        Hide();
        SetPhysicsProcess(false);
        Freeze = true;
    }

    public void Destroy()
    {
        QueueFree();
    }

    public abstract ShipType GetShipType();
    
    protected void ReCalculateEnergy()
    {
        _energyPercent = CalculateEnergyPercent();
    }
    
    private int CalculateEnergyPercent()
    {
        int percent = 0;

        foreach (BaseGenerator generator in GetGenerators())
        {
            percent += generator.GetEnergyPercent();
        }

        if (percent > 100)
        {
            percent = 100;
        }

        return percent;
    }

    private List<BaseGenerator> GetGenerators()
    {
        List<BaseGenerator> result = new();

        Array<Node> slots = GeneratorSlots.GetChildren();

        foreach (Node slot in slots)
        {
            BaseGenerator generator = slot.GetChildCount() == 1
                ? slot.GetChild<BaseGenerator>(0)
                : null;

            if (generator == null)
            {
                continue;
            }

            result.Add(generator);
        }

        return result;
    }

    private List<BaseGun> GetGuns()
    {
        List<BaseGun> result = new();
        Array<Node> slots = GunSlots.GetChildren();

        return result;
    }

    public override void _PhysicsProcess(double delta)
    {
        _pid = new(Clamp(_maxSpeed, 1, 30) * 0.5f, 0, 2.0f);
        _maxSpeed = World.GetMaxSpeed();
        int controlResponsibility = GetControlResponsibilityPercent();
        float shift = _maxShift * Clamp((float)controlResponsibility / 100, 0.7f, 2f);

        Speed = Lerp(Speed, _maxSpeed, 0.01f);

        _direction = new Vector3(
            GetActionStrength("move_right") - GetActionStrength("move_left"),
            GetActionStrength("move_forward") - GetActionStrength("move_backward"),
            -1
        );

        float controlResponsibilityY = Clamp(controlResponsibility / 100, 0.5f, 1f);
        float controlResponsibilityX = Clamp(controlResponsibility / 100, 0.5f, 1.5f);

        Vector3 updatedDirection = new Vector3(
            Abs(shift - Abs(LinearVelocity.X)) * _direction.X,
            _direction.Y * _gravity * 0.5f * controlResponsibilityY,
            Abs(Speed - Abs(LinearVelocity.Z)) * _direction.Z
        );

        float rotateEnergy = 4.0f;

        Vector3 rotation = Rotation;

        if (_direction.Y != 0)
        {
            bool isYLocked = !(
                (GlobalPosition.Y <= _maxHeight || _direction.Y < 0)
                && (GlobalPosition.Y >= _minHeight || _direction.Y > 0)
            );
            AxisLockLinearY = isYLocked;
            if (!isYLocked)
            {
                rotation.X = Lerp(
                    Rotation.X,
                    DegToRad(_direction.Y * _maxXRotation),
                    (float)delta * (rotateEnergy * 0.7f) * controlResponsibilityY
                );
            }
            else
            {
                rotation.X = Lerp(
                    Rotation.X,
                    DegToRad(0),
                    (float)delta * (rotateEnergy / 2) * controlResponsibilityY
                );
            }
        }
        else
        {
            AxisLockLinearY = true;
            rotation.X = Lerp(
                Rotation.X,
                DegToRad(0),
                (float)delta * (rotateEnergy / 2) * controlResponsibilityY
            );
        }

        if (_direction.X != 0)
        {
            rotation.Z = Lerp(
                Rotation.Z,
                DegToRad(-_direction.X * _maxZRotation),
                (float)delta * rotateEnergy * controlResponsibilityX
            );
        }
        else
        {
            updatedDirection.X = -LinearVelocity.X * 0.9f;
            rotation.Z = Lerp(
                Rotation.Z,
                DegToRad(0),
                (float)delta * (rotateEnergy / 2) * controlResponsibilityX
            );
        }

        Rotation = rotation;

        Vector3 targetVelocity = updatedDirection * SPEED;
        Vector3 velocityError = targetVelocity - LinearVelocity;
        _correctionImpulse = _pid.Update(velocityError, (float)delta) * 0.001f;

        ApplyCentralImpulse(_correctionImpulse);

        Player.Position = GlobalPosition;
        Player.Speed = get_current_speed();
    }

    private float get_current_speed()
    {
        return Abs(LinearVelocity.Z);
    }

    private void _prepare()
    {
        Slicer1.Transform = DeviateSlicer(Slicer1.Transform);
        Slicer2.Transform = DeviateSlicer(Slicer2.Transform);
        _meshes1 = (Array<ArrayMesh>)Slicer.Call("slice_mesh", Slicer1.Transform, Mesh.Mesh);
        _meshes2 = (Array<ArrayMesh>)Slicer.Call("slice_mesh", Slicer2.Transform, _meshes1[0]) +
                   (Array<ArrayMesh>)Slicer.Call("slice_mesh", Slicer2.Transform, _meshes1[1]);
    }

    private Transform3D DeviateSlicer(Transform3D transform)
    {
        transform.Origin *= new Vector3(
            (float)RandRange(0.6, 1.4),
            1,
            (float)RandRange(0.6, 1.4)
        );
        return transform.Rotated(new Vector3(1, 1, 1).Normalized(), DegToRad(0));
    }

    private async void Destroy(CollisionObject3D collider)
    {
        Explosion01.Play();

        SetCollisionMaskValue((int)CollisionType.Wall, false);
        SetCollisionMaskValue((int)CollisionType.Laser, false);

        bool isLaser = collider.CollisionLayer == (int)CollisionType.Laser;

        if (isLaser)
        {
            await ToSignal(GetTree().CreateTimer(0.2), SceneTreeTimer.SignalName.Timeout);
        }

        SetPhysicsProcess(false);
        Freeze = true;
        Visible = false;

        if (isLaser)
        {
            SpawnWreck(_meshes1[0]);
            SpawnWreck(_meshes1[1]);
        }
        else
        {
            SpawnWreck(_meshes2[0]);
            SpawnWreck(_meshes2[1]);
            SpawnWreck(_meshes2[2]);
            SpawnWreck(_meshes2[3]);
        }

        Player.Destroy();
    }

    private async void SpawnWreck(ArrayMesh slicedMesh)
    {
        RigidBody3D rigid = new RigidBody3D();
        MeshInstance3D mesh = new MeshInstance3D();
        CollisionShape3D shape = new CollisionShape3D();
        mesh.Transform = Mesh.Transform;
        shape.Transform = Shape.Transform;
        mesh.Mesh = slicedMesh;
        CopyMaterial(Mesh, mesh);
        shape.Shape = mesh.Mesh.CreateConvexShape();
        rigid.TopLevel = true;
        rigid.SetCollisionMaskValue((int)CollisionType.Wall, true);
        rigid.SetCollisionMaskValue((int)CollisionType.Tile, true);
        rigid.AddChild(mesh);
        rigid.AddChild(shape);

        AddChild(rigid);
        rigid.Reparent(World);

        rigid.ApplyCentralImpulse(new Vector3(
            RandRange(-3, 3),
            RandRange(-1, 2),
            RandRange(-3, -6))
        );
        await ToSignal(GetTree().CreateTimer(10), SceneTreeTimer.SignalName.Timeout);
        rigid.QueueFree();
    }

    private void CopyMaterial(MeshInstance3D from, MeshInstance3D to)
    {
        int c = Min(from.GetSurfaceOverrideMaterialCount(), to.GetSurfaceOverrideMaterialCount());
        foreach (int i in Enumerable.Range(0, c - 1))
        {
            to.SetSurfaceOverrideMaterial(i, from.GetActiveMaterial(i));
        }
    }

    public void OnBodyEntered(CollisionObject3D body)
    {
        if (body.CollisionLayer == (int)CollisionType.TunnelEnter)
        {
            body.QueueFree();
            Camera.GetIntoTunnel();
            return;
        }

        if (body.CollisionLayer == (int)CollisionType.TunnelExit)
        {
            body.QueueFree();
            Camera.GetOutFromTunnel();
            return;
        }

        Destroy(body);
    }

    public void OnEngineSoundFinished()
    {
        Engine01.Play();
    }

    public abstract void UpgradeEnergy();
}