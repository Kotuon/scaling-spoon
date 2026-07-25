using Godot;

public partial class ChunkPlacer : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadChunks();

        SetupPlayer();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    protected void SetupPlayer()
    {
        var pos_list = Global.Instance.PlayerStartPos;
        if (pos_list == null)
        {
            GD.PushError("Bad list.");
            return;
        }

        if (pos_list.ContainsKey(Global.Instance.CurrentScene.Name))
        {
            var player = GetNode<Node2D>("YSort/Player");
            player.GlobalPosition = pos_list[GetTree().CurrentScene.Name];
        }
    }

    protected void LoadChunks()
    {
        var tilemaps = GetNode("TileMaps").GetChildren();
        var sort = GetNode("YSort");

        foreach (var tilemap in tilemaps)
        {
            if (tilemap is not Area2D)
                continue;

            var chunk = tilemap as Area2D;
            var ground = tilemap.GetNode<TileMapLayer>("Ground");
            var detail = tilemap.GetNode<TileMapLayer>("Detail");
            detail.Name += chunk.Name;

            ground.ZIndex = -1;

            chunk.RemoveChild(detail);
            sort.AddChild(detail);

            detail.GlobalPosition = chunk.GlobalPosition;
            detail.Scale *= Scale * chunk.Scale;

            if (!tilemap.HasNode("YSort"))
                continue;
            var child_ysort = tilemap.GetNode<Node2D>("YSort");

            var ysort_children = child_ysort.GetChildren();
            foreach (var child in ysort_children)
            {
                var global_pos = (child as Node2D).GlobalPosition;
                var global_scale = (child as Node2D).GlobalScale;

                child_ysort.RemoveChild(child);
                sort.AddChild(child);

                (child as Node2D).GlobalPosition = global_pos;
                (child as Node2D).GlobalScale = global_scale;
            }
        }
    }
}
