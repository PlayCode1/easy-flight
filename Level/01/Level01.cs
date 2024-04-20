using System.Collections.Generic;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._03;
using App.Tile._06;
using Godot;

namespace App.Level._01;

public partial class Level01 : BaseLevel
{
    public override int GetNumber()
    {
        return 1;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/01/boss_level_01.tscn"),
        };
    }

    public override int GetMaxSpeed()
    {
        return 30;
    }

    public override int GetDistance()
    {
        return 50;
    }

    public override List<IPackedTile> GetTiles()
    {
        return new List<IPackedTile>()
        {
            new PackedTile01(),
            new PackedTile06(),
            new PackedTile03(),
        };
    }

    public override void Spawn(Node3D tile)
    {
    }
}