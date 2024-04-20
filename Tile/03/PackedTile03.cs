using Godot;

namespace App.Tile._03;

public class PackedTile03: BasePackageTile
{
    public override float GetMaxPlayerSpeed()
    {
        return 100f;
    }

    public override float GetMinPlayerSpeed()
    {
        return 0f;
    }

    public override bool IsSupportPlayerSpawn()
    {
        return false;
    }

    public override int GetIndex()
    {
        return 3;
    }

    public override int GetRarity()
    {
        return 60;
    }
}