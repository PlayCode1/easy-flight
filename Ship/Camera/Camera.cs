using Godot;
using static Godot.Mathf;

namespace App.Ship.Camera;

public partial class Camera: Camera3D
{
    private const float Smoothness = 1.5f;

    Node3D Player => GetParent<Node3D>();
    private Node3D TargetCamera => GetParent().GetNode<Node3D>("TargetCamera");
    private Node3D TargetTunnelCamera => GetParent().GetNode<Node3D>("TargetTunnelCamera");
    private Node3D _target;
    private Score Score => GetNode<Score>("Score");
    private Speed Speed => GetNode<Speed>("Speed");
    private bool _inTunnel = false;

    public override void _Ready()
    {
        GetOutFromTunnel();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Player == null)
        {
            return;
        }

        float w = (float)delta * Smoothness;

        GlobalPosition = _inTunnel ? new Vector3(
            _target.GlobalPosition.X,
            Lerp(GlobalPosition.Y, _target.GlobalPosition.Y, w),
            _target.GlobalPosition.Z
        ) : _target.GlobalPosition;

        GlobalRotation = new Vector3(
            Lerp(GlobalRotation.X, _target.GlobalRotation.X, w), 
            Lerp(GlobalRotation.Y, _target.GlobalRotation.Y, w), 
            Lerp(GlobalRotation.Z, _target.GlobalRotation.Z, w) 
        );
    }

    public void OnTimerTimeout()
    {
        Score.Update();
        Speed.Update();
    }

    public async void GetIntoTunnel()
    {
        // TODO: Tunnel is not implemented
        // _target = TargetTunnelCamera;
        // _inTunnel = true;
    }

    public async void GetOutFromTunnel()
    {
        _target = TargetCamera;

        // TODO: Tunnel is not implemented
        // await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
        // _inTunnel = false;
    }
}