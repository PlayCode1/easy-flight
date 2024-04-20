using System;
using Godot;
using Godot.Collections;

namespace App.Audio;

public abstract partial class BaseAudioPlayer : Node3D
{
    [Export] public Array<AudioStream> sounds;
    [Export] public App.Audio.Audio.BusEnum bus;
    [Export] public float maxInterval = 0;

    private IPlayer _player;
    private int _currentTrack = 0;

    public interface IPlayer
    {
        void SetBus(StringName bus);
        public void Change(AudioStream stream);
        public void Play();
        public void Pitch(float value);
    }

    protected abstract IPlayer CreatePlayer();
    protected abstract void DoAfterFinished();

    public override void _Ready()
    {
        _player = CreatePlayer();
        _player.SetBus(Enum.GetName(bus));
        Next();
    }

    public async void OnFinished()
    {
        await ToSignal(
            GetTree().CreateTimer(GD.Randf() % maxInterval),
            SceneTreeTimer.SignalName.Timeout
        );

        DoAfterFinished();
    }

    public void Next()
    {
        if (_currentTrack == 0)
        {
            Shuffle();
        }

        if (_currentTrack == sounds.Count)
        {
            _currentTrack = 0;
        }

        _player.Change(sounds[_currentTrack]);
        _player.Play();
        _currentTrack++;
    }

    public void Mute()
    {
        int busIndex = (int)bus;
        AudioServer.SetBusMute(busIndex, !AudioServer.IsBusMute(busIndex));
    }

    public void Pitch(float value)
    {
        _player.Pitch(value);
    }

    protected void Shuffle()
    {
        Random random = new Random();
        int n = sounds.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (sounds[k], sounds[n]) = (sounds[n], sounds[k]);
        }
    }
}