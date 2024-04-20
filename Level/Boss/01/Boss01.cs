using Godot;

namespace App.Level.Boss._01;

public partial class Boss01: BaseBoss
{
    private bool _upgraded = false;

    protected override int GetHealth()
    {
        return 100;
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