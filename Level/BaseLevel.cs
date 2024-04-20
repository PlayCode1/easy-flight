using System.Collections.Generic;
using App.Tile;
using Godot;

namespace App.Level;

public abstract partial class BaseLevel : Node3D
{
    protected MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);
    private int _traveledDistance = 0;
    private bool _isCompleted = false;

    public abstract int GetNumber();

    public abstract List<PackedScene> GetPackedBosses();

    public abstract int GetMaxSpeed(); // Max 100 (80)

    public abstract int GetDistance();

    public abstract List<IPackedTile> GetTiles();

    public abstract void Spawn(Node3D tile);

    public virtual void AddTraveledDistance()
    {
        if (GetDistance() == 0)
        {
            return;
        }

        if (!_isCompleted && GetTraveledDistancePercent() >= 100)
        {
            Bus.EmitSignal(MessageBus.SignalName.LevelComplete);
            _isCompleted = true;
            return;
        }

        _traveledDistance += 1;
    }

    public int GetTraveledDistancePercent()
    {
        return (int)((float)_traveledDistance / GetDistance() * 100);
    }
}