using Godot;

[Tool]
public partial class ChildNodeSave : Node
{
    [Export]
    public bool trigger_script
    {
        set
        {
            if (value) Run();
        }
        get => false;
    }
    private void Run()
    {
        GD.Print("Hello from the Godot Editor!");

        var children = GetChildren();

        foreach (var child in children)
        {
            var name = "res://levels/" + GetParent().Name + "/" + child.Name + ".tscn";
            if (ResourceLoader.Exists(name)) continue;

            // GD.Print(name);
            PackedScene scene = new();
            scene.Pack(child);
            ResourceSaver.Save(scene, name);
        }
    }
}
