using System;
using Godot;

namespace App.Tile;

public abstract class BasePackageTile: IPackedTile
{
    private readonly PackedScene _packedScene;

    public BasePackageTile()
    {
        string file = String.Format(
            "res://Tile/{0:D2}/Tile_{0:D2}.tscn", GetIndex()
        );
        _packedScene = GD.Load<PackedScene>(file);
    }

    public abstract float GetMaxPlayerSpeed();

    public abstract float GetMinPlayerSpeed();

    public abstract bool IsSupportPlayerSpawn();

    public abstract int GetIndex();

    public abstract int GetRarity();

    public PackedScene GetPacked()
    {
        return _packedScene;
    }
}