namespace App.Tile._02;

public class PackedTile02: BasePackageTile
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
        return 2;
    }

    public override int GetRarity()
    {
        return 20;
    }
}