using Godot;

namespace App;

public partial class MessageBus : Node
{
    [Signal]
    public delegate void AddLeftTileEventHandler(Node3D tile);

    [Signal]
    public delegate void AddTileByZEventHandler(Node3D tile);

    [Signal]
    public delegate void AddRightTileEventHandler(Node3D tile);

    [Signal]
    public delegate void DestroyTileEventHandler(Vector3 position);

    [Signal]
    public delegate void LevelCompleteEventHandler();

    [Signal]
    public delegate void BossFightCompleteEventHandler();

    [Signal]
    public delegate void NewRecordEventHandler(int value);

    [Signal]
    public delegate void LastLevelEventHandler(int value);

    [Signal]
    public delegate void DebugAddTileEventHandler();

    [Signal]
    public delegate void DebugRemoveTileEventHandler();

    [Signal]
    public delegate void PlayerDestroyedEventHandler();
}