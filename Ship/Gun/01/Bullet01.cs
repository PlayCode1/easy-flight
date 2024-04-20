namespace App.Ship.Gun._01;

public partial class Bullet01: BaseBullet
{
    public override int GetDamage()
    {
        return 10;
    }

    protected override int GetLifeTimeInSec()
    {
        return 6;
    }
}