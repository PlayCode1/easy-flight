using Godot;

namespace App.Ship._01;

public partial class Ship01 : BaseShip
{
    private enum GunSlotType
    {
        Slot1,
        Slot2,
    }

    private PackedScene PackedGenerator => GD.Load<PackedScene>("res://Ship/Generator/01/generator_01.tscn");

    public override void _Ready()
    {
        base._Ready();
    }

    protected override int GetControlResponsibilityPercent()
    {
        return 100;
    }

    public override ShipType GetShipType()
    {
        return ShipType.Ship1;
    }

    public override void UpgradeEnergy()
    {
        Node slot2 = GeneratorSlots.GetChild<Node>((int)EnergySlotType.Slot2);
        Node slot3 = GeneratorSlots.GetChild<Node>((int)EnergySlotType.Slot3);

        if (slot3.GetChildCount() == 1)
        {
            return;
        }
        
        if (slot2.GetChildCount() == 0)
        {
            slot2.AddChild(PackedGenerator.Instantiate());
            Mesh.SetSurfaceOverrideMaterial(2, EnergyMaterial);
        }
        else
        {
            slot3.AddChild(PackedGenerator.Instantiate());
            Mesh.SetSurfaceOverrideMaterial(3, EnergyMaterial);
        }

        ReCalculateEnergy();
    }
}