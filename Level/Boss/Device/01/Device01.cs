using Godot;

namespace App.Level.Boss.Device._01;

public partial class Device01: Node3D
{
    private Player Player => GetNode<Player>(SingletonPath.Player);
    private bool _ready = false;
    private PackedScene Bullet => GD.Load<PackedScene>("res://Level/Boss/Device/01/boss_bullet_01.tscn");

    public override async void _Ready()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
        ProcessMode = ProcessModeEnum.Inherit;
        Fire();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_ready == false && Position.Y > 1.06f)
        {
            _ready = true;
        }

        if (_ready)
        {
            return;
        }

        Position = new Vector3(
            Position.X,
            Position.Y + 1f * (float) delta,
            Position.Z
        );
    }

    private async void Fire()
    {
        if (_ready)
        {
            AddChild(Bullet.Instantiate());
        }

        if (!IsInsideTree())
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);

        Fire();
    }
}