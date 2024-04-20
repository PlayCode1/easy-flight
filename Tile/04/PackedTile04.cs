namespace App.Tile._04;

public class PackedTile04: BasePackageTile
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
        return 4;
    }

    public override int GetRarity()
    {
        return 80;
    }
}