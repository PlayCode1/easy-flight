using Godot;

namespace App.Level.Boss._04;

public partial class Boss04: BaseBoss
{
    private bool _upgraded = false;

    protected override int GetHealth()
    {
        return 2000;
    }

    protected override void OnDamage()
    {
    }

    protected override void OnKilled()
    {
        if (_upgraded)
        {
            return;
        }

        Player.UpgradeShipEnergy();
        _upgraded = true;
    }
}