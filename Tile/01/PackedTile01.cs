
namespace App.Tile._01;

public class PackedTile01: BasePackageTile
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
        return true;
    }

    public override int GetIndex()
    {
        return 1;
    }

    public override int GetRarity()
    {
        return 0;
    }
}