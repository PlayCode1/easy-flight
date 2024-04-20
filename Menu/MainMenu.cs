using Godot;

namespace App.Menu;

public partial class MainMenu: Control
{
    private PackedScene _mainMenu => GD.Load<PackedScene>("res://Menu/main_menu.tscn");
    private PackedScene _level1 => GD.Load<PackedScene>("res://Level/01/Level01.tscn");

    public void OnButtonStartPressed()
    {
        GetTree().ChangeSceneToPacked(_level1);
    }

    public void QuitToMainMenu()
    {
        GetTree().ChangeSceneToPacked(_mainMenu);
        Show();
    }

    public void Quit()
    {
        GetTree().Quit();
    }
}