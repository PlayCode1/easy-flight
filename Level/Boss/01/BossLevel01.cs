using System.Collections.Generic;
using App.Tile;
using App.Tile._01;
using App.Tile._06;
using Godot;

namespace App.Level.Boss._01;

public partial class BossLevel01 : BaseBossLevel
{
    private PackedScene Boss => GD.Load<PackedScene>("res://Level/Boss/01/boss_01.tscn");

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
        };
    }

    public override void Spawn(Node3D tile)
    {
        // Nothing
    }
}