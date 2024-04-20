using Godot;

namespace App.Level.Boss._05;

public partial class Boss05: BaseBoss
{
    private bool _upgraded = false;

    protected override int GetHealth()
    {
        return 2500;
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