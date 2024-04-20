using App.Audio.Sound;
using Godot;

namespace App.Ship.Gun;

public abstract partial class BaseBullet : CharacterBody3D
{
    [Export] public AudioStream ExplosionSound;
    public int Speed;

    private Player Player => GetNode<Player>(SingletonPath.Player);
    private PackedScene Explosion => GD.Load<PackedScene>("res://Explosion.tscn");

    public abstract int GetDamage();
    protected abstract int GetLifeTimeInSec();

    public override async void _Ready()
    {
        await ToSignal(GetTree().CreateTimer(GetLifeTimeInSec()), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = -(GlobalTransform.Basis.Z * Speed * (float)delta + GlobalTransform.Basis.Z * Player.Speed);
        MoveAndSlide();
    }

    public void OnWall(Node3D body)
    {
        Node3D node = (Node3D)Explosion.Instantiate();
        body.AddChild(node);
        node.GlobalPosition = GlobalPosition;
        new SoundPlayerSpawner(ExplosionSound, body, 1).Spawn();
        Destroy();
    }

    public void Destroy()
    {
        SetPhysicsProcess(false);
        QueueFree();
    }
}