using Godot;

namespace App.Tile._06;

public partial class Tile_06 : BaseTile
{
    public override void _Ready()
    {
        base._Ready();
        Rotate90();
    }
}