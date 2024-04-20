using System.Collections.Generic;
using App.Component.Zeppelin;
using App.Tile;
using App.Tile._01;
using App.Tile._02;
using App.Tile._03;
using App.Tile._06;
using Godot;

namespace App.Level._03;

public partial class Level03 : BaseLevel
{
    private PackedScene PackedZeppelin => GD.Load<PackedScene>("res://Component/Zeppelin/Zeppelin.tscn");

    public override int GetNumber()
    {
        return 3;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/03/boss_level_03.tscn"),
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
            new PackedTile06(),
            new PackedTile02(),
            new PackedTile03(),
        };
    }

    public override void Spawn(Node3D tile)
    {

        if (GD.Randi() % 101 > 90)
        {
            Zeppelin zeppelin = (Zeppelin)PackedZeppelin.Instantiate();
            zeppelin.Position = new Vector3(
                zeppelin.Position.X,
                zeppelin.Position.Y + 20,
                zeppelin.Position.Z
            );
            tile.AddChild(zeppelin);
        }
    }
}