using Godot;

namespace App.Ship.Camera;

public partial class Speed : MeshInstance3D
{
    private Player Player => GetNode<Player>(SingletonPath.Player);

    public void Update()
    {
        TextMesh textMesh = (TextMesh)Mesh;
        textMesh.Text = ((int)Player.Speed).ToString();
    }
}