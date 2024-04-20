using Godot;
using System;
using App.World;

namespace App.Component.Tower.Gun;

public partial class Gun01 : Node3D
{
    private const float AttackDistance = 1.5f;
    private bool _enabled = false;

    private Node3D Parent => GetParent<Node3D>();
    private AnimationTree Tree => GetNode<AnimationTree>("AnimationTree");
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    private CollisionObject3D Socket => GetNode<CollisionObject3D>("Armature/Skeleton3D/Gun/Gun/Socket");
    private AudioStreamPlayer3D FirePlayer => GetNode<AudioStreamPlayer3D>("Audio/Fire");

    private int _gunHorn = 1;
    private bool _standBy = false;
    private bool _readyToFire = false;
    private Player Player => GetNode<Player>(SingletonPath.Player);

    public override void _Ready()
    {
        AnimationPlayer.Play("Prepare", -1d, 1f);
    }

    public override void _PhysicsProcess(double delta)
    {
        float playerX = Player.Position.X;
        float playerZ = Player.Position.Z;
        float playerSpeed = Player.Speed * AttackDistance;
        float distanceX = Math.Abs(GlobalPosition.X - playerX);
        float distanceZ = Math.Abs(GlobalPosition.Z - playerZ);

        if (!(distanceX < playerSpeed && distanceZ < playerSpeed * 2))
        {
            return;
        }

        if (!_standBy && !AnimationPlayer.IsPlaying())
        {
            _standBy = true;
            _readyToFire = true;
            if (_enabled)
            {
                AnimationPlayer.Play("Fire");
            }
        }

        if (_standBy && _readyToFire)
        {
            _Fire();
        }
    }

    public void Enable()
    {
        _enabled = true;
    }

    private async void _Fire()
    {
        _readyToFire = false;
        Socket.SetCollisionLayerValue((int)CollisionType.Laser, true);
        Socket.Visible = true;

        await ToSignal(GetTree().CreateTimer(2), SceneTreeTimer.SignalName.Timeout);

        _readyToFire = true;
        Socket.SetCollisionLayerValue((int)CollisionType.Laser, false);
        Socket.Visible = false;
        SetPhysicsProcess(false);

        await ToSignal(GetTree().CreateTimer(2), SceneTreeTimer.SignalName.Timeout);

        SetPhysicsProcess(true);
    }
}