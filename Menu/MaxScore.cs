using Godot;

namespace App.Menu;

public partial class MaxScore : Label
{
    private Player Player => GetNode<Player>(SingletonPath.Player);

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        NewRecord(Player.HighScore > Player.Score ? Player.HighScore : Player.Score);
    }

    private void NewRecord(int value)
    {
        Text = $"HIGH SCORE: {value}";
    }
}