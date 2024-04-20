using Godot;

namespace App.Level.Boss._02;

public partial class Boss02: BaseBoss
{
    private bool _upgraded = false;

    protected override int GetHealth()
    {
        return 300;
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