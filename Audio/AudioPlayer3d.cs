using Godot;

namespace App.Audio;

public partial class AudioPlayer3d : BaseAudioPlayer
{
    protected override async void DoAfterFinished()
    {
        await ToSignal(GetTree().CreateTimer(2), SceneTreeTimer.SignalName.Timeout);
        Next();
    }

    protected override IPlayer CreatePlayer()
    {
        return new Player(GetNode<AudioStreamPlayer3D>("Player3D"));
    }
    
    public class Player: IPlayer
    {
        private readonly AudioStreamPlayer3D _player;

        public Player(AudioStreamPlayer3D player)
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