using System.Collections.Generic;
using App.Component.Zeppelin;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._03;
using App.Tile._04;
using App.Tile._05;
using App.Tile._06;
using Godot;

namespace App.Level._05;

public partial class Level05 : BaseLevel
{
    public override int GetNumber()
    {
        return 5;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/05/boss_level_05.tscn"),
        };
    }

    public override int GetMaxSpeed()
    {
        return 60;
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
            new PackedTile05(),
        };
    }

    public override void Spawn(Node3D tile)
    {
    }
}