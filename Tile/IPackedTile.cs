using System;
using Godot;

namespace App.Tile;

public interface IPackedTile
{
    public float GetMaxPlayerSpeed();

    public float GetMinPlayerSpeed();

    public bool IsSupportPlayerSpawn();

    public int GetIndex();

    // 0 - 100
    public int GetRarity();

    public PackedScene GetPacked();
}