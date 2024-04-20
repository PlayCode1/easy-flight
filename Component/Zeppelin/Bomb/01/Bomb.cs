using Godot;

namespace App.Component.Zeppelin.Bomb._01;

public partial class Bomb: Node3D
{
    private Player Player => GetNode<Player>(SingletonPath.Player);
    private Shield Shield => GetNode<Shield>("../Shield");
    private bool _isPlaying = false;

    public void OnTimerTimeout()
    {
        var maxHeight = Player.MaxHeight - GD.RandRange(0.0, Player.MaxHeight - 1);

        if (_isPlaying || !(GlobalPosition.Y < maxHeight))
        {
            return;
        }

        _isPlaying = true;
        Shield.Activate(GlobalPosition);

        QueueFree();
    }
}

