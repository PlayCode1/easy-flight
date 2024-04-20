using Godot;

namespace App.Ship.Generator;

public abstract partial class BaseGenerator: Node3D
{
    public abstract int GetEnergyPercent();
}