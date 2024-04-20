using System;
using Godot;

namespace App.Menu;

public partial class GameMenu : Control
{
    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);
    private Player Player => GetNode<Player>(SingletonPath.Player);
    private bool _paused = true;

    public override void _Ready()
    {
        bool paused = Player.GetSave()[Player.SaveSetting.ShowMenu].ToInt() == 1;

        _paused = paused;

        if (paused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed)
            {
                if (eventKey.Keycode == Key.Escape)
                {
                    Pause();
                }

                if (eventKey.Keycode == Key.Q)
                {
                    if (_paused)
                    {
                        Quit();
                    }
                }
            }
        }
    }

    public void Pause()
    {
        if (_paused)
        {
            _paused = false;
            Hide();
        }
        else
        {
            _paused = true;
            Show();
        }
        
        Player.Save(Player.SaveSetting.ShowMenu, _paused ? "1" : "0");
    }

    public void Quit()
    {
        GetTree().Quit();
    }
}