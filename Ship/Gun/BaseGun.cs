using App.Audio.Sound;
using Godot;

namespace App.Ship.Gun;

public abstract partial class BaseGun: Node3D
{
    [Export] public AudioStream FireSound;
    private const float MinInterval = 0.6f;
    private BaseShip Ship => GetParent().GetParent().GetParent().GetParent<BaseShip>();
    private float _interval;
    private bool _canFire = true;
    private bool _soundLock = false;

    protected abstract PackedScene GetPackedBullet();
    protected abstract int GetBulletSpeed();
    protected abstract int GetEfficiencyPercent();
    protected abstract float GetSoundLevel();

    public override void _Ready()
    {
        // Update(); // TODO: BaseShip AllGun Update
        OnFireEnable();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_canFire && Input.IsMouseButtonPressed(MouseButton.Left))
        {
            Fire();
            _canFire = false;
        }
    }


    public void Update()
    {
        _interval = CalculateInterval();
    }

    protected void Fire()
    {
        BaseBullet bullet = (BaseBullet)GetPackedBullet().Instantiate();

        if (!_soundLock)
        {
            _soundLock = true;
            new SoundPlayerSpawner(FireSound, this, GetSoundLevel()).Spawn();
            UnlockSound();
        }

        bullet.Speed = GetBulletSpeed();
        AddChild(bullet);
    }

    private async void OnFireEnable()
    {
        _canFire = true;
        Update();
        await ToSignal(
            GetTree().CreateTimer(_interval), 
            SceneTreeTimer.SignalName.Timeout
        );
        OnFireEnable();
    }

    private float CalculateInterval()
    {
        int power = Mathf.Min(GetEfficiencyPercent(), Ship.GetEnergyPercent());
        
        return Mathf.Clamp(MinInterval * Mathf.Sqrt((float) 100 / power), 0.1f, 2f);
    }

    private async void UnlockSound()
    {
        await ToSignal(GetTree().CreateTimer(0.2), SceneTreeTimer.SignalName.Timeout);
        _soundLock = false;
    }
}