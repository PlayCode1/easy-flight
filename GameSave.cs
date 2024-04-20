using Godot;

namespace App;

public partial class GameSave : Resource
{
    private const string SavePath = "user://save.tres";

    public int MaxScore = 0;

    public void Save()
    {
        ResourceSaver.Save(this, SavePath);
    }

    public Resource Load()
    {
        if (!ResourceLoader.Exists(SavePath))
        {
            return null;
        }

        return ResourceLoader.Load(SavePath);
    }
}