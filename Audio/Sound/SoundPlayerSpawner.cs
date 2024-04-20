using System;
using Godot;

namespace App.Audio.Sound;

public partial class SoundPlayerSpawner : Node3D
{
    private readonly AudioStream _audioStream;
    private readonly Node3D _node;
    private readonly float _volume;

    public SoundPlayerSpawner(
        AudioStream audioStream,
        Node3D node,
        float volume = 0f
    )
    {
        _audioStream = audioStream;
        _node = node;
        _volume = volume;
    }

    public AudioStreamPlayer3D Spawn()
    {
        AudioStreamPlayer3D player = CreateSoundPlayer();
        _node.AddChild(player);

        return player;
    }

    private AudioStreamPlayer3D CreateSoundPlayer()
    {
        AudioStreamPlayer3D player = new AudioStreamPlayer3D();
        player.ProcessMode = ProcessModeEnum.Inherit;
        player.Stream = _audioStream;
        player.VolumeDb = _volume;
        player.PitchScale = 1.0f;
        player.UnitSize = 10.0f;
        player.MaxDistance = 30;
        player.Bus = Enum.GetName(Audio.BusEnum.Sound);
        player.Autoplay = true;

        return player;
    }
}