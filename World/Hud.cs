using System.Globalization;
using Godot;

namespace App;

public partial class Hud : Node
{
	MessageBus mb;
	private int _debugSpawned = 0;

	public override void _Ready()
	{
		mb = GetNode<MessageBus>("/root/MessageBus");
		mb.DebugAddTile += AddTile;
		mb.DebugRemoveTile += RemoveTile;
		mb.PlayerDestroyed += OnPlayerDestroyed;
	}

	public override void _Process(double delta)
	{
		GetNode<Label>("FPS").Text = Engine.GetFramesPerSecond().ToString(CultureInfo.CurrentCulture);
	}

	private void AddTile()
	{
		_debugSpawned++;
		GetNode<Label>("TILES").Text = _debugSpawned.ToString();
	}

	private void RemoveTile()
	{
		_debugSpawned--;
		GetNode<Label>("TILES").Text = _debugSpawned.ToString();
	}

	private void OnPlayerDestroyed()
	{
		mb.DebugAddTile -= AddTile;
		mb.DebugRemoveTile -= RemoveTile;
		mb.PlayerDestroyed -= OnPlayerDestroyed;
	}
}
