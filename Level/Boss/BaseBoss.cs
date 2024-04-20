using App.Ship.Gun;
using Godot;
using App.Audio.Sound;

namespace App.Level.Boss;

public abstract partial class BaseBoss : CharacterBody3D
{
    [Export] public AudioStream DamageSound;
    [Export] public AudioStream FireSound;
    [Export] public AudioStream KillSound;
    protected Player Player => GetNode<Player>(SingletonPath.Player);
    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);
    private Material DamageMaterial => GD.Load<Material>("res://Material/Laser.tres");
    private MeshInstance3D Boss => GetNode<MeshInstance3D>("Boss");
    private int _health;
    private bool _killed = false;
    private bool _soundLock = false;

    protected abstract int GetHealth();

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = new Vector3(
            Mathf.Lerp(GlobalPosition.X, Player.Position.X, (float)delta * 1.7f),
            Mathf.Lerp(GlobalPosition.Y, Player.Position.Y, (float)delta * 1.7f),
            Player.Position.Z - 10
        );
    }

    public override void _Ready()
    {
        _health = GetHealth();
    }

    public void OnBodyEntered(BaseBullet bullet)
    {
        AddDamage(bullet.GetDamage());
        bullet.Destroy();
    }

    protected abstract void OnDamage();

    protected abstract void OnKilled();

    private async void AddDamage(int value)
    {
        if (_killed)
        {
            return;
        }

        _health -= value;

        Boss.SetSurfaceOverrideMaterial(0, DamageMaterial);
        if (!_soundLock)
        {
            _soundLock = true;
            new SoundPlayerSpawner(DamageSound, this, 1).Spawn();
            UnlockSound();
        }

        await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        if (_killed)
        {
            return;
        }

        Boss.SetSurfaceOverrideMaterial(0, null);
        OnDamage();

        if (!_killed && _health <= 0)
        {
            _killed = true;
            Boss.SetSurfaceOverrideMaterial(0, DamageMaterial);
            new SoundPlayerSpawner(KillSound, this, 1).Spawn();

            await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);

            if (!Player.IsMaxUpgrade())
            {
                OnKilled();
            }

            QueueFree();

            Bus.EmitSignal(MessageBus.SignalName.BossFightComplete);
        }
    }

    private async void UnlockSound()
    {
        await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
        _soundLock = false;
    }
}