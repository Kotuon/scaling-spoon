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

        var cells = GetUsedCells();

        foreach (var cell_id in cells)
        {
            var cell = GetCellTileData(cell_id);

            (cell.Material as ShaderMaterial).SetShaderParameter(
                "player_pos", player_ref.GetGlobalTransformWithCanvas().Origin);
        }

        // (Material as ShaderMaterial).SetShaderParameter(
        //     "player_pos", player_ref.GetGlobalTransformWithCanvas().Origin);


    }

    public override void _Draw()
    {
        base._Draw();

        // var cells = GetUsedCells();

        // foreach (var cell_id in cells)
        // {
        //     var cell = GetCellTileData(cell_id);

        //     var pos = MapToLocal(cell_id);

        //     DrawCircle(new Vector2(pos.X, pos.Y + (float)cell.YSortOrigin), 5.0f, new Color(1.0f, 0.0f, 0.0f));
        // }
    }
}
