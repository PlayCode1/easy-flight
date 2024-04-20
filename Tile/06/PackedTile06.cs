namespace App.Tile._06;

public class PackedTile06: BasePackageTile
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
        return 6;
    }

    public override int GetRarity()
    {
        return 0;
    }
}