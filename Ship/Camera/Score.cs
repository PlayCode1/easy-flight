using Godot;

namespace App.Ship.Camera;

public partial class Score: MeshInstance3D
{
    private Player Player => GetNode<Player>(SingletonPath.Player);
    private MessageBus Bus => GetNode<MessageBus>(SingletonPath.MessageBus);

    public void Update()
    {
        TextMesh textMesh = (TextMesh)Mesh;
        textMesh.Text = Player.Score.ToString();
    }
}