using App.World;
using Godot;

namespace App.Tile._03;

public partial class Laser: StaticBody3D
{
    private const float INTERVAL = 1.7f;

    public override void _Ready()
    {
        Toggle();
    }

    private async void Toggle()
    {
        Visible = !Visible;
        SetCollisionLayerValue((int)CollisionType.Laser, Visible);

        if (!IsInsideTree())
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(INTERVAL + GD.RandRange(0f, 2f)), SceneTreeTimer.SignalName.Timeout);
        Toggle();
    }
}