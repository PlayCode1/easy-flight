using System.Collections.Generic;
using Godot;
using Godot.Collections;


namespace App.Tile;

public partial class BaseTile : Node3D
{
    [Export] public Node3D Mesh;

    protected Node3D parent;
    protected MeshInstance3D mesh;
    protected MessageBus messageBus;

    public override void _Ready()
    {
        parent = GetParent<Node3D>();
        mesh = GetNode<MeshInstance3D>("Platform");
        messageBus = GetNode<MessageBus>("/root/MessageBus");

        messageBus.DestroyTile += OnDestroyTile;

        messageBus.EmitSignal(MessageBus.SignalName.DebugAddTile);
    }

    private void OnDestroyTile(Vector3 position)
    {
        if (position == parent.Position)
        {
            messageBus.DestroyTile -= OnDestroyTile;
            messageBus.EmitSignal(MessageBus.SignalName.DebugRemoveTile);
            parent.QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    public void Destroy()
    {
        GetParent().QueueFree();
    }

    public void OnForwardExited(RigidBody3D body)
    {
        messageBus.EmitSignal(MessageBus.SignalName.AddTileByZ, GetParent());
    }

    public void OnRightExited(RigidBody3D body)
    {
        if (body.LinearVelocity.X <= 0)
        {
            return;
        }

        messageBus.EmitSignal(MessageBus.SignalName.AddRightTile, GetParent());
    }

    public void OnLeftExited(RigidBody3D body)
    {
        if (body.LinearVelocity.X > 0)
        {
            return;
        }

        messageBus.EmitSignal(MessageBus.SignalName.AddLeftTile, GetParent());
    }

    public void OnTimerTimeout()
    {
    }

    protected void Rotate180()
    {
        parent.RotationDegrees = new Vector3(
            0,
            new Array<int>() { 0, 180 }[GD.RandRange(0, 1)],
            0
        );
    }

    protected void Rotate90()
    {
        parent.RotationDegrees = new Vector3(
            0,
            new Array<int>() { 0, 90, 180, 270 }[GD.RandRange(0, 3)],
            0
        );
    }
}