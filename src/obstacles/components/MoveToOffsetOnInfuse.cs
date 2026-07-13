namespace Game.Component;

using Godot;

[Tool]
public partial class MoveToOffsetOnInfuse : MoveToOffset
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        enabled = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!enabled)
            return;
    }

    public override void _Draw()
    {
        base._Draw();
    }
}
