using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using App.Audio;
using App.Level;
using App.Ship;
using App.Tile;
using Godot;

namespace App.World;

public partial class World : Node3D
{
    private const float DeleteAfterTileCount = 6;

    private Player Player => GetNode<Player>(SingletonPath.Player);
    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);
    private ColorRect RetroFilter => GetNode<ColorRect>("UI/Retro");
    private ProgressBar DistanceProgressBar => GetNode<ProgressBar>("UI/VBoxContainer/DistanceProgressBar");
    private PackedScene _tile01;
    private Vector3 _tileSize;
    private readonly List<Vector3> _spawnedTiles = new();
    private bool _spawnLock = false;
    private Stack<Node3D> _stack = new();
    private AudioPlayer _musicPlayer;
    private AudioPlayer _ambientPlayer;
    private Node3D _currentTile;
    private BaseLevel _currentLevel;
    private int _currentBossNumber;
    private DisplayServer.WindowMode _defaultWindowMode = DisplayServer.WindowMode.Fullscreen;

    private Dictionary<int, PackedScene> Levels => new()
    {
        { 1, GD.Load<PackedScene>("res://Level/01/level_01.tscn") },
        { 2, GD.Load<PackedScene>("res://Level/02/level_02.tscn") },
        { 3, GD.Load<PackedScene>("res://Level/03/level_03.tscn") },
        { 4, GD.Load<PackedScene>("res://Level/04/level_04.tscn") },
        { 5, GD.Load<PackedScene>("res://Level/05/level_05.tscn") },
        { 6, GD.Load<PackedScene>("res://Level/06/level_06.tscn") },
    };

    private List<IPackedTile> _packedTiles = new();
    private BaseShip ship;

    public override void _Ready()
    {
        DisplayServer.WindowSetTitle("Easy Flight");
        UpdateScreenSettings();
        Engine.MaxFps = 60;

        if (Player.GetSave()[Player.SaveSetting.GameMode] == Player.GetGameModeIndex(Player.GameMode.Retro).ToString())
        {
            RetroFilter.Show();
        }

        Node3D tile01 = GetNode<Node3D>("Tile_01");

        _tileSize = GetNode<MeshInstance3D>("Tile_01/BaseTile/Platform").Scale;

        Bus.AddLeftTile += SpawnToLeft;
        Bus.AddTileByZ += SpawnByZ;
        Bus.AddRightTile += SpawnToRight;
        Bus.PlayerDestroyed += OnPlayerDestroyed;
        Bus.LevelComplete += OnLevelComplete;
        Bus.BossFightComplete += OnBossFightComplete;

        Start();
        ToSpawned(tile01.Position);
        Spawn(tile01);
    }

    public int GetMaxSpeed()
    {
        return _currentLevel.GetMaxSpeed();
    }

    private void Start()
    {
        Player.SelectShip(BaseShip.ShipType.Ship1);
        SelectLevel(Player.LevelNumber);

        ship = Player.GetShipFromInventory(Player.SelectedShipType);
        Player.ToCockpit(ship);
        AddChild(ship);
        ship.GlobalPosition = Player.Position;
    }

    private void SelectLevel(int number)
    {
        DirectionalLight3D currentLight = new DirectionalLight3D();
        if (_currentLevel != null)
        {
            currentLight = _currentLevel.GetNode<DirectionalLight3D>("DirectionalLight3D");
        }

        _currentLevel?.QueueFree();
        _currentLevel = null;

        BaseLevel level = (BaseLevel)Levels[number].Instantiate();

        AddChild(level);
        _ambientPlayer = level.GetNode<AudioPlayer>("Audio/AmbientPlayer");
        _musicPlayer = level.GetNode<AudioPlayer>("Audio/MusicPlayer");
        _currentLevel = level;
        _packedTiles = _currentLevel.GetTiles();

        if (currentLight != null)
        {
            DirectionalLight3D targetLight = level.GetNode<DirectionalLight3D>("DirectionalLight3D");
            Object[] current = { currentLight.LightColor, currentLight.LightEnergy };
            Object[] target = { targetLight.LightColor, targetLight.LightEnergy };
            TransitionToLevel(level, current, target);
        }
    }

    private async void TransitionToLevel(BaseLevel level, Object[] from, Object[] to)
    {
        if (level != _currentLevel)
        {
            return;
        }

        Color color = ((Color)from[0]).Lerp((Color)to[0], 0.1f);
        float energy = Mathf.Lerp((float)from[1], (float)to[1], 0.1f);
        from[0] = color;
        from[1] = energy;

        level.GetNode<DirectionalLight3D>("DirectionalLight3D").LightColor = color;
        level.GetNode<DirectionalLight3D>("DirectionalLight3D").LightEnergy = energy;

        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

        if (from[0] != to[0])
        {
            TransitionToLevel(level, from, to);
        }
    }

    private void SelectBoss()
    {
        DirectionalLight3D currentLight = _currentLevel.GetNode<DirectionalLight3D>("DirectionalLight3D");
        List<PackedScene> packedBosses = _currentLevel.GetPackedBosses();

        if (packedBosses.Count == 0)
        {
            OnBossFightComplete();
            return;
        }

        _currentLevel?.QueueFree();
        _currentLevel = null;

        BaseLevel boss = (BaseLevel)packedBosses[GD.RandRange(0, packedBosses.Count - 1)].Instantiate();
        AddChild(boss);
        _ambientPlayer = boss.GetNode<AudioPlayer>("Audio/AmbientPlayer");
        _musicPlayer = boss.GetNode<AudioPlayer>("Audio/MusicPlayer");
        _currentLevel = boss;
        _packedTiles = _currentLevel.GetTiles();
        DirectionalLight3D targetLight = boss.GetNode<DirectionalLight3D>("DirectionalLight3D");
        Object[] current = { currentLight.LightColor, currentLight.LightEnergy };
        Object[] target = { targetLight.LightColor, targetLight.LightEnergy };
        TransitionToLevel(boss, current, target);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stack.Count == 0)
        {
            return;
        }

        Node3D tile = _stack.Pop();

        // TODO: Bug
        if (!IsInstanceValid(tile))
        {
            return;
        }

        SpawnTiles(tile);
        _stack.Clear();
    }

    private async void OnPlayerDestroyed()
    {
        GD.Print("World: Player is destroyed");

        Bus.AddLeftTile -= SpawnToLeft;
        Bus.AddTileByZ -= SpawnByZ;
        Bus.AddRightTile -= SpawnToRight;
        Bus.PlayerDestroyed -= OnPlayerDestroyed;
        Bus.LevelComplete -= OnLevelComplete;
        Bus.BossFightComplete -= OnBossFightComplete;

        _musicPlayer.Pitch(0.6f);

        await ToSignal(GetTree().CreateTimer(6f), SceneTreeTimer.SignalName.Timeout);

        Player.ResetState();
        ClearTiles();
        Start();
        GetTree().ReloadCurrentScene();
    }

    private void OnLevelComplete()
    {
        SelectBoss();
    }

    private async void OnBossFightComplete()
    {
        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);

        Player.NextLevel();
        SelectLevel(Player.LevelNumber);
    }

    private void SpawnToLeft(Node3D tile)
    {
        Spawn(tile);
    }

    private void SpawnByZ(Node3D tile)
    {
        Spawn(tile);

        _currentLevel.AddTraveledDistance();
        DistanceProgressBar.SetValueNoSignal(_currentLevel.GetTraveledDistancePercent());
    }

    private void SpawnToRight(Node3D tile)
    {
        Spawn(tile);
    }

    private void Spawn(Node3D tile)
    {
        Player.AddScore();

        _stack.Push(tile);
    }

    private void ClearTiles(Node3D? tile = null)
    {
        List<LambdaExpression> destroyList = new List<LambdaExpression>();
        _spawnedTiles.ForEach(spawnedTilePosition =>
        {
            if (tile == null || spawnedTilePosition.Z > tile.Position.Z + DeleteAfterTileCount * _tileSize.X)
            {
                destroyList.Add(() => _spawnedTiles.Remove(spawnedTilePosition));
                Bus.EmitSignal(MessageBus.SignalName.DestroyTile, spawnedTilePosition);
            }
        });
        destroyList.ForEach(destroy => destroy.Compile().DynamicInvoke());
    }

    private void SpawnTiles(Node3D tile)
    {
        CallDeferred(MethodName.ClearTiles, tile);

        if (_spawnLock)
        {
            return;
        }

        _spawnLock = true;

        int[,] area =
        {
            { 2, 1, 1, 0, 0, 0, 0, 0 }, // 2 Player
            { 1, 1, 1, 1, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
        };

        Vector3 size = _tileSize;

        List<Vector3> spawnPoints = new();

        for (int z = 0; z < area.GetLength(0); z++)
        {
            for (int x = 0; x < area.GetLength(1); x++)
            {
                int type = area[z, x];

                if (type == 0)
                {
                    continue;
                }

                spawnPoints.Add(new Vector3(
                    tile.Position.X + size.X * x,
                    0,
                    tile.Position.Z - size.Z * z
                ));

                if (x > 0)
                {
                    spawnPoints.Add(new Vector3(
                        tile.Position.X - size.X * x,
                        0,
                        tile.Position.Z - size.Z * z
                    ));
                }
            }
        }

        int i = 0;
        int firstTileCount = area.GetLength(0) * 10;

        foreach (Vector3 point in spawnPoints)
        {
            if (!_spawnedTiles.Contains(point))
            {
                List<IPackedTile> packedTiles = SelectPackedTiles(Player.Speed);
                List<IPackedTile> firstPackedTiles = packedTiles.Where(
                    (IPackedTile packedTile) => packedTile.IsSupportPlayerSpawn()
                ).ToList();

                SpawnTile(
                    i <= firstTileCount && Player.LevelNumber == 1
                        ? firstPackedTiles[(int)(GD.Randi() % firstPackedTiles.Count)]
                        : packedTiles[(int)(GD.Randi() % packedTiles.Count)],
                    point
                );
            }

            i++;
        }

        _spawnLock = false;
        _currentTile = tile;
    }

    private List<IPackedTile> SelectPackedTiles(float playerSpeed)
    {
        List<IPackedTile> list = new List<IPackedTile>();

        foreach (var packedTile in _packedTiles)
        {
            if (playerSpeed > packedTile.GetMaxPlayerSpeed())
            {
                continue;
            }

            if (playerSpeed < packedTile.GetMinPlayerSpeed())
            {
                continue;
            }

            if (GD.Randi() % 101 < packedTile.GetRarity())
            {
                continue;
            }

            list.Add(packedTile);
        }

        if (list.Count == 0)
        {
            list.Add(_packedTiles[0]);
        }

        return list;
    }

    private void SpawnTile(IPackedTile tile, Vector3 target)
    {
        ToSpawned(target);
        Node3D instance = (Node3D)tile.GetPacked().Instantiate();
        instance.Translate(target);

        _currentLevel.Spawn(instance);

        AddChild(instance);
    }

    private void ToSpawned(Vector3 tile)
    {
        _spawnedTiles.Add(tile);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.Pressed)
            {
                if (eventKey.Keycode == Key.F)
                {
                    if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                    {
                        SaveWindowMode(DisplayServer.WindowMode.Windowed);
                        UpdateScreenSettings();
                    }
                    else
                    {
                        SaveWindowMode(DisplayServer.WindowMode.Fullscreen);
                        UpdateScreenSettings();
                    }
                }

                if (eventKey.Keycode == Key.R)
                {
                    if (RetroFilter.Visible)
                    {
                        RetroFilter.Hide();
                        Player.Save(
                            Player.SaveSetting.GameMode,
                            Player.GetGameModeIndex(Player.GameMode.Standard).ToString()
                        );
                    }
                    else
                    {
                        RetroFilter.Show();
                        Player.Save(
                            Player.SaveSetting.GameMode,
                            Player.GetGameModeIndex(Player.GameMode.Retro).ToString()
                        );
                    }
                }
            }
        }

        if (@event.IsActionPressed("next_track"))
        {
            _musicPlayer.Next();
        }

        if (@event.IsActionPressed("mute_music"))
        {
            _musicPlayer.Mute();
        }
    }

    private void UpdateScreenSettings()
    {
        DisplayServer.WindowMode windowMode = _defaultWindowMode;

        windowMode = Player.GetSave()[Player.SaveSetting.FullScreen] == "1"
            ? DisplayServer.WindowMode.Fullscreen
            : DisplayServer.WindowMode.Windowed;

        DisplayServer.WindowSetMode(windowMode);
        DisplayServer.MouseSetMode(windowMode == DisplayServer.WindowMode.Fullscreen
            ? DisplayServer.MouseMode.Captured
            : DisplayServer.MouseMode.Visible);
    }

    private void SaveWindowMode(DisplayServer.WindowMode windowMode)
    {
        Player.Save(
            Player.SaveSetting.FullScreen,
            windowMode == DisplayServer.WindowMode.Fullscreen ? "1" : "0"
        );
    }
}