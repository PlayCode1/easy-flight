using Godot;

namespace App.Ship.Gun._01;

public partial class Gun01 : BaseGun
{
    protected override PackedScene GetPackedBullet()
    {
        return GD.Load<PackedScene>("res://Ship/Gun/01/bullet_01.tscn");
    }

    protected override int GetBulletSpeed()
    {
        return 500;
    }

    protected override int GetEfficiencyPercent()
    {
        return 50;
    }

    protected override float GetSoundLevel()
    {
        return -21f;
    }
}