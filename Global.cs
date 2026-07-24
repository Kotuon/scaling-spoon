using Godot;

public partial class Global : Node
{
    public static Global Instance { get; private set; }
    public int my_test = 0;

    public Node CurrentScene { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;

        Viewport root = GetTree().Root;
        CurrentScene = root.GetChild(-1);
    }

    public void GotoScene(string path)
    {
        CallDeferred(MethodName.DeferredGotoScene, path);
    }

    public void DeferredGotoScene(string path)
    {
        CurrentScene.Free();

        var next_scene = GD.Load<PackedScene>(path);
        CurrentScene = next_scene.Instantiate();

        GetTree().Root.AddChild(CurrentScene);
        GetTree().CurrentScene = CurrentScene;
    }
}
