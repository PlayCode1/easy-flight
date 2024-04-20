using System.Collections.Generic;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._06;
using Godot;

namespace App.Level._02;

public partial class Level02 : BaseLevel
{
    public override int GetNumber()
    {
        return 2;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/02/boss_level_02.tscn"),
        };
    }

    public override int GetMaxSpeed()
    {
        return 40;
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
            new PackedTile06(),
            new PackedTile02(),
        };
    }

    public override void Spawn(Node3D tile)
    {
    }
}