namespace Game.Entity;

using Godot;

public partial class DetailLayer : TileMapLayer
{
    private Player player_ref;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player_ref = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);

        (Material as ShaderMaterial).SetShaderParameter(
            "player_pos", player_ref.GetGlobalTransformWithCanvas().Origin);
    }
}
