
namespace App.Tile._05;

public class PackedTile05: BasePackageTile
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
        return 5;
    }

    public override int GetRarity()
    {
        return 90;
    }
}