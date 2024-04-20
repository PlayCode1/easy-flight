using Godot;

namespace App.Ship.Gun._02;

public partial class Gun02: BaseGun
{
    protected override PackedScene GetPackedBullet()
    {
        return GD.Load<PackedScene>("res://Ship/Gun/02/bullet_02.tscn");
    }

    protected override int GetBulletSpeed()
    {
        return 1000;
    }

    protected override int GetEfficiencyPercent()
    {
        return 75;
    }

    protected override float GetSoundLevel()
    {
        return -25f;
    }
}