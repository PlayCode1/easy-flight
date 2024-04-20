using System.Collections.Generic;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._03;
using App.Tile._04;
using App.Tile._06;
using Godot;

namespace App.Level._04;

public partial class Level04 : BaseLevel
{
    public override int GetNumber()
    {
        return 4;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/04/boss_level_04.tscn"),
        };
    }

    public override int GetMaxSpeed()
    {
        return 50;
    }

    public override int GetDistance()
    {
        return 100;
    }

    public override List<IPackedTile> GetTiles()
    {
        return new List<IPackedTile>()
        {
            new PackedTile01(),
            new PackedTile01(),
            new PackedTile06(),
            new PackedTile06(),
            new PackedTile02(),
            new PackedTile03(),
            new PackedTile04(),
        };
    }

    public override void Spawn(Node3D tile)
    {
        // Nothing
    }
}