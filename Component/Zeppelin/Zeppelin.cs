using Godot;

namespace App.Component.Zeppelin;

public partial class Zeppelin : CharacterBody3D
{
    private const float Speed = 170f;

    [Export] public AudioStream BombAudioStream;

    private Player Player => GetNode<Player>(SingletonPath.Player);
    private Bomb._01.Bomb Bomb => GetNode<Bomb._01.Bomb>("Bomb");
    private AudioStreamPlayer3D _bombAudioPlayer;
    private int _bombCount = 1;

    public override void _Ready()
    {
        RotateY(Mathf.DegToRad(GD.Randi() % 360));
        Position = new Vector3(
            Position.X,
            Position.Y * (float)GD.RandRange(0.7, 1.2),
            Position.Z
        );
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 playerPosition = Player.Position;
        var playerSpeed = Player.Speed;
        var distance = GlobalPosition.DistanceTo(playerPosition);
        if (_bombCount > 0 && distance < Mathf.Clamp(playerSpeed * 4, 20, 600))
        {
            _DropBomb();
        }

        Velocity = -GlobalTransform.Basis.X * Speed * (float)GD.RandRange(0.5, 2) * (float)delta;
        MoveAndSlide();
    }

    private void _DropBomb()
    {
        _bombCount -= 1;
        Bomb.Visible = true;
        Bomb.ProcessMode = ProcessModeEnum.Inherit;
        Bomb.TopLevel = true;
    }
}