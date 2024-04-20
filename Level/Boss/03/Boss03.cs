using App.Ship;
using Godot;

namespace App.Level.Boss._03;

public partial class Boss03: BaseBoss
{
    private bool _upgraded = false;

    protected override int GetHealth()
    {
        return 500;
    }

    protected override void OnDamage()
    {
    }

    protected override async void OnKilled()
    {
        if (_upgraded)
        {
            return;
        }

        BaseShip current = Player.GetCurrentShip();
        Node3D parent = current.GetParent<Node3D>();

        Player.SelectShip(BaseShip.ShipType.Ship2);
        BaseShip ship = Player.GetShipFromInventory(Player.SelectedShipType);
        Player.ToCockpit(ship);
        parent.AddChild(ship);
        ship.Speed = 60;
        ship.GlobalPosition = Player.Position;
     
        _upgraded = true;
        
        current.Stop();
        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        current.Destroy();
    }
}