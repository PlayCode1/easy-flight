namespace App.Ship.Gun._02;

public partial class Bullet02: BaseBullet
{
    public override int GetDamage()
    {
        return 20;
    }

    protected override int GetLifeTimeInSec()
    {
        return 6;
    }
}