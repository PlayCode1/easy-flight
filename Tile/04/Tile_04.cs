using App.Audio.Sound;
using Godot;

namespace App.Tile._04;

public partial class Tile_04 : BaseTile
{
    [Export] public AudioStream Sound;
    [Export] public float AlarmVolume = 0;

    public override void _Ready()
    {
        base._Ready();
        Rotate90();

        CallDeferred(MethodName.Alarm);
    }

    private void Alarm()
    {
        new SoundPlayerSpawner(Sound, this, AlarmVolume).Spawn();
    }
}