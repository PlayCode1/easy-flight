using System;
using App.Ship;
using Godot;
using Godot.Collections;
using static App.Ship.BaseShip;

namespace App;

public partial class Player : Node
{
    public enum SaveSetting
    {
        HighScore,
        FullScreen,
        ShowMenu,
        GameMode,
    }

    public enum GameMode
    {
        Standard,
        Retro,
    }

    private const int MaxLevel = 6;

    private readonly Vector3 _defaultPosition = new Vector3(0, 3, 3);

    private Dictionary<SaveSetting, string> GetDefaultSave()
    {
        return new()
        {
            { SaveSetting.HighScore, "0" },
            { SaveSetting.FullScreen, "1" },
            { SaveSetting.ShowMenu, "1" },
            { SaveSetting.GameMode, GetGameModeIndex(GameMode.Standard).ToString() },
        };
    }

    private Dictionary<ShipComponent, EnergySlotType> GetDefaultInventoryShip()
    {
        return new()
        {
            {
                ShipComponent.Energy, EnergySlotType.Slot1
            },
        };
    }

    // Dynamic
    public float MinHeight;
    public float MaxHeight;
    public Vector3 Position;
    public float Speed = 0f;
    public int LevelNumber = 1;
    public int Score = 0;

    // Save
    public int HighScore = 0;

    public Dictionary<ShipType, Dictionary<ShipComponent, EnergySlotType>> Ships;

    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);

    private Dictionary<ShipType, PackedScene> PackedShips => new()
    {
        { ShipType.Ship1, GD.Load<PackedScene>("res://Ship/01/ship_01.tscn") },
        { ShipType.Ship2, GD.Load<PackedScene>("res://Ship/02/ship_02.tscn") },
    };

    private string _savePath = "user://player.save";
    private BaseShip _ship = null;
    public ShipType SelectedShipType;

    public Player()
    {
        Ships = new()
        {
            {
                ShipType.Ship1, GetDefaultInventoryShip()
            },
            {
                ShipType.Ship2, GetDefaultInventoryShip()
            },
        };

        Position = _defaultPosition;
        
        FixOldSave();
        HighScore = GetSave()[SaveSetting.HighScore].ToInt();
    }

    public Dictionary<SaveSetting, string> GetSave()
    {
        Dictionary<SaveSetting, string> save = GetDefaultSave();

        if (FileAccess.FileExists(_savePath))
        {
            FileAccess read = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
            save = (Dictionary<SaveSetting, string>) read.GetVar();
            read.Close();
        }

        return save;
    }

    public void Save(SaveSetting name, string value)
    {
        Dictionary<SaveSetting, string> save = GetDefaultSave();

        if (FileAccess.FileExists(_savePath))
        {
            FileAccess read = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
            save = (Dictionary<SaveSetting, string>) read.GetVar();
            read.Close();
        }

        FileAccess file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
        save[name] = value;
        file.StoreVar(save);
        file.Close();
    }

    public void FixOldSave()
    {
        if (FileAccess.FileExists(_savePath))
        {
            FileAccess read = null;

            try
            {
                read = FileAccess.Open(_savePath, FileAccess.ModeFlags.Read);
                Dictionary<SaveSetting, string> save = (Dictionary<SaveSetting, string>)read.GetVar();
                var a = save[SaveSetting.FullScreen];
                var b = save[SaveSetting.HighScore];
                var c = save[SaveSetting.GameMode];
                var d = save[SaveSetting.ShowMenu];
            }
            catch (Exception e)
            {
                if (read != null)
                {
                    read.Close();
                }
                
                FileAccess file = FileAccess.Open(_savePath, FileAccess.ModeFlags.Write);
                file.StoreVar(GetDefaultSave());
                file.Close();
                
            }
            finally
            {
                if (read != null)
                {
                    read.Close();
                }
            }
        }
    }

    public int GetGameModeIndex(GameMode mode)
    {
        if (mode == GameMode.Retro)
        {
            return 1;
        }

        return 0;
    }

    public GameMode GetGameModeByIndex(int index)
    {
        if (index == 1)
        {
            return GameMode.Retro;
        }

        return GameMode.Standard;
    }

    public void NextLevel()
    {
        if (LevelNumber == MaxLevel)
        {
            return;
        }

        LevelNumber += 1;
    }

    public void SelectShip(ShipType shipType)
    {
        SelectedShipType = shipType;
    }

    public void ToCockpit(BaseShip ship)
    {
        _ship = ship;
        SelectedShipType = ship.GetShipType();
    }

    public BaseShip GetCurrentShip()
    {
        return _ship;
    }

    public bool IsMaxUpgrade()
    {
        return LevelNumber == MaxLevel;
    }

    public void AddShipToInventory(ShipType shipType)
    {
        Ships.Add(
            shipType,
            new Dictionary<ShipComponent, EnergySlotType>() { { ShipComponent.Energy, EnergySlotType.Slot1 } }
        );
    }

    public BaseShip GetShipFromInventory(ShipType shipType)
    {
        if (Ships[shipType] == null)
        {
            throw new ArgumentException("Not exist");
        }

        BaseShip ship = PackedShips[shipType].Instantiate<BaseShip>();

        EnergySlotType slot = Ships[shipType][ShipComponent.Energy];

        if (slot == EnergySlotType.Slot2)
        {
            ship.UpgradeEnergy();
        }

        if (slot == EnergySlotType.Slot3)
        {
            ship.UpgradeEnergy();
            ship.UpgradeEnergy();
        }

        return ship;
    }

    public void UpgradeShipEnergy()
    {
        if (_ship == null)
        {
            return;
        }

        ShipType shipType = _ship.GetShipType();

        if (Ships[shipType] == null)
        {
            return;
        }

        EnergySlotType slot = Ships[shipType][ShipComponent.Energy];

        if (slot == EnergySlotType.Slot3)
        {
            return;
        }

        Ships[shipType][ShipComponent.Energy] = slot == EnergySlotType.Slot2
            ? EnergySlotType.Slot3
            : EnergySlotType.Slot2;

        _ship.UpgradeEnergy();
    }

    public void ResetState()
    {
        LevelNumber = 1;
        Score = 0;

        Ships = new()
        {
            { ShipType.Ship1, GetDefaultInventoryShip() },
            { ShipType.Ship2, GetDefaultInventoryShip() },
        };
    }

    public void Destroy()
    {
        if (Score > HighScore)
        {
            SetNewRecord(Score);
        }

        Score = 0;
        Position = _defaultPosition;

        Bus.EmitSignal(MessageBus.SignalName.PlayerDestroyed);
    }

    public void AddScore()
    {
        Score += 10;
    }

    public void AddSuperScore()
    {
        Score += 1000;
    }

    public void SetNewRecord(int score)
    {
        HighScore = score;
        Save(SaveSetting.HighScore, score.ToString());

        // TODO: Not implemented
        // Bus.EmitSignal(MessageBus.SignalName.NewRecord, MaxScore);
    }
}