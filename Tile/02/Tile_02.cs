using App.Component.Tower.Gun;
using Godot;

namespace App.Tile._02;

public partial class Tile_02 : BaseTile
{
    private PackedScene _cube;
    private PackedScene _gun;
    private Node3D container;
    private RigidBody3D _player;

    public override void _Ready()
    {
        base._Ready();
        _cube = GD.Load<PackedScene>("res://Component/Tower/Tower.tscn");
        _gun = GD.Load<PackedScene>("res://Component/Tower/Gun/Gun01.tscn");
        container = GetNode<Node3D>("Cubes");

        Rotate90();
        
        CallDeferred(MethodName.Spawn);
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    private void Spawn()
    {
        int assCubes = 1;
        for (int z = 0; z < mesh.Scale.Z; z++)
        {
            for (int x = 0; x < mesh.Scale.X; x++)
            {
                if (assCubes > 0 && GD.RandRange(1, 10) > 9)
                {
                    float height = GD.Randf() * 12;
                    Node3D iCube = (Node3D)_cube.Instantiate();
                    Gun01 iGun = (Gun01)_gun.Instantiate();

                    iCube.Translate(new Vector3(
                        x - mesh.Scale.X / 2 + iCube.Scale.X / 2,
                        height + iCube.Position.Y,
                        z - mesh.Scale.Z / 2 + iCube.Scale.Z / 2
                    ));

                    assCubes--;

                    container.AddChild(iCube);
                    iCube.AddChild(iGun);

                    if (height > 7)
                    {
                        iGun.Enable();
                    }
                }
                else
                {
                    // instance.Scale = new Vector3(1, GD.Randf() * 2 + 1, 1);
                }
            }
        }
    }
}