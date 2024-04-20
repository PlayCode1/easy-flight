using App.Audio.Sound;
using Godot;

namespace App.Component.Zeppelin.Bomb._01;

public partial class Shield: StaticBody3D
{
    [Export] public AudioStream AudioStream;
    [Export] public float Volume = 0;

    public void Activate(Vector3 globalPosition)
    {
        Visible = true;
        ProcessMode = ProcessModeEnum.Inherit;
        float scale = (float)GD.RandRange(0.7, 3.0);
        Scale = new Vector3(scale, scale, scale);
        GlobalPosition = globalPosition;

        new SoundPlayerSpawner(AudioStream, this, Volume).Spawn();
    }
}