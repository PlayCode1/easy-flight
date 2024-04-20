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

namespace App.Level._06;

public partial class Level06 : Level.BaseLevel
{
    private PackedScene PackedZeppelin => GD.Load<PackedScene>("res://Component/Zeppelin/Zeppelin.tscn");
    private bool _zeppelin = false;

    public Level06()
    {
        if (GD.Randi() % 101 > 70)
        {
            _zeppelin = true;
        }
    }

    public override void _Ready()
    {
        GetNode<DirectionalLight3D>("DirectionalLight3D").LightColor = new Color(
            Mathf.Clamp(GD.Randf() % 1f, 0.2f, 0.7f),
            Mathf.Clamp(GD.Randf() % 1f, 0.2f, 0.7f),
                Mathf.Clamp(GD.Randf() % 1f, 0.2f, 0.7f)
        );
        GetNode<DirectionalLight3D>("DirectionalLight3D").LightEnergy = GD.Randf() % 1f + 0.5f;
    }

    public override int GetNumber()
    {
        return 6;
    }

    public override List<PackedScene> GetPackedBosses()
    {
        return new List<PackedScene>
        {
            GD.Load<PackedScene>("res://Level/Boss/01/boss_level_01.tscn"),
            GD.Load<PackedScene>("res://Level/Boss/02/boss_level_02.tscn"),
            GD.Load<PackedScene>("res://Level/Boss/03/boss_level_03.tscn"),
            GD.Load<PackedScene>("res://Level/Boss/04/boss_level_04.tscn"),
            GD.Load<PackedScene>("res://Level/Boss/05/boss_level_05.tscn"),
        };
    }

    public override int GetMaxSpeed()
    {
        return (int)(GD.Randi() % 40) + 20;
    }

    public override int GetDistance()
    {
        return 150;
        // return (int)(GD.Randi() % 100) + 100;
    }

    public override List<IPackedTile> GetTiles()
    {
        List<IPackedTile> all = new List<IPackedTile>()
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
        
        List<IPackedTile> list = new List<IPackedTile>() { };

        for (int i = 0; i < 5; i++)
        {
            if (GD.Randi() % 101 > 60)
            {
                list.Add(all[i]);
            }
        }

        if (list.Count <= 2)
        {
            list = all;
        }

        return list;
    }

    public override void Spawn(Node3D tile)
    {
        if (_zeppelin && GD.Randi() % 101 > 90)
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