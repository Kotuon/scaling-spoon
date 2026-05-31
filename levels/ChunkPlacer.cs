using Godot;

public partial class ChunkPlacer : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var tilemaps = GetNode("TileMaps").GetChildren();
        var sort = GetNode("YSort");

        foreach (var tilemap in tilemaps)
        {
            if (tilemap is not Area2D) continue;

            var chunk = tilemap as Area2D;
            // var ground = tilemap.GetNode<TileMapLayer>("Ground");
            var detail = tilemap.GetNode<TileMapLayer>("Detail");
            detail.Name += chunk.Name;

            chunk.RemoveChild(detail);
            sort.AddChild(detail);

            detail.GlobalPosition = chunk.GlobalPosition;
            detail.Scale *= Scale * chunk.Scale;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
