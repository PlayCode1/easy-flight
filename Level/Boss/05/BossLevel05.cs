using System.Collections.Generic;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._03;
using App.Tile._04;
using App.Tile._05;
using App.Tile._06;
using Godot;

namespace App.Level.Boss._05;

public partial class BossLevel05 : BaseBossLevel
{
    private PackedScene Boss => GD.Load<PackedScene>("res://Level/Boss/05/boss_05.tscn");

    public override void _Ready()
    {
        AddChild(Boss.Instantiate());
    }

    public override int GetNumber()
    {
        return 0;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return null;
    }

    public override int GetMaxSpeed()
    {
        return 30;
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
            new PackedTile04(),
            new PackedTile05(),
        };
    }

    public override void Spawn(Node3D tile)
    {
        // Nothing
    }
}