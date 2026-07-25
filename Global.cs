using Game.Entity;
using Godot;
using Godot.Collections;

public partial class Global : Node
{
    public static Global Instance { get; private set; }
    public int my_test = 0;

    public Node CurrentScene { get; set; }

    public Dictionary<string, Vector2> PlayerStartPos = [];
    public Dictionary<string, bool> CanLevelChange = [];

    public Dictionary<string, PackedScene> Levels = [];

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;

        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(-1);

        Levels.Add(
            "res://levels/main_world.tscn",
            GD.Load<PackedScene>("res://levels/main_world.tscn")
        );
    }

    public void GotoScene(
        string path,
        CharacterBase player,
        bool savePlayerPos = false
    )
    {
        if (savePlayerPos)
        {
            PlayerStartPos[GetTree().CurrentScene.Name] = player.GlobalPosition;
        }

        CallDeferred(MethodName.DeferredGotoScene, path);
    }

    public void DeferredGotoScene(string path)
    {
        CurrentScene.Free();

        if (!Levels.ContainsKey(path))
        {
            var next_scene = GD.Load<PackedScene>(path);
            CurrentScene = next_scene.Instantiate();
        }
        else
        {
            CurrentScene = Levels[path].Instantiate();
        }

        GetTree().Root.AddChild(CurrentScene);
        GetTree().CurrentScene = CurrentScene;
    }
}
