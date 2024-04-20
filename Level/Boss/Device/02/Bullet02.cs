using Godot;

namespace App.Level.Boss.Device._02;

public partial class Bullet02 : CharacterBody3D
{
    private const float Speed = 150;
    private const int LifeTime = 6;
    private Player Player => GetNode<Player>(SingletonPath.Player);

    public override async void _Ready()
    {
        GlobalRotation = new Vector3(
            Mathf.DegToRad((int)(GD.Randi() % 20) - 10),
            Mathf.DegToRad((int)(GD.Randi() % 40) - 20),
            Rotation.Z
        );
        await ToSignal(GetTree().CreateTimer(LifeTime), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = GlobalTransform.Basis.Z * Speed * (float)delta - GlobalTransform.Basis.Z * Player.Speed;
        MoveAndSlide();
    }
}