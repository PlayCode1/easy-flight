namespace App.World;

public enum CollisionType
{
    Wall = 1,
    Laser = 2,
    Tile = 32,
    Bullet = 64,
    Player = 512,
    TunnelEnter = 1024,
    TunnelExit = 2048,
}