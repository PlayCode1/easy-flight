using Godot;

namespace App.Audio;

public partial class AudioPlayer : BaseAudioPlayer
{
    protected override void DoAfterFinished()
    {
        Next();
    }

    protected override IPlayer CreatePlayer()
    {
        return new Player(GetNode<AudioStreamPlayer>("Player"));
    }

    public class Player: IPlayer
    {
        private readonly AudioStreamPlayer _player;

        public Player(AudioStreamPlayer player)
        {
            _player = player;
        }

        public void SetBus(StringName bus)
        {
            _player.Bus = bus;
        }

        public void Change(AudioStream stream)
        {
            _player.Stream = stream;
        }

        public void Play()
        {
            _player.Play();
        }

        public void Pitch(float value)
        {
            _player.PitchScale = value;
        }
    }
}