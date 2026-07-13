namespace Game.Component;

using Game.Entity;
using Godot;

public partial class ObstacleComponent : Node2D, IAutoDoor
{
    [Export]
    protected Godot.Collections.Array<Area2D> keys = [];

    protected Obstacle _parent = null;

    protected Obstacle parent
    {
        private set => _parent = value;
        get
        {
            if (_parent == null && GetParent() is Obstacle)
                _parent = GetParent() as Obstacle;
            return _parent;
        }
    }

    private bool _enabled = true;
    [Export]
    public bool enabled
    {
        set => _enabled = value;
        get => _enabled;
    }

    public override void _Ready()
    {
        base._Ready();

        GetParent<Area2D>().BodyEntered += ResolveCollisionEnter;
        GetParent<Area2D>().BodyExited += ResolveCollisionExit;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!enabled && keys.Count > 0)
            enabled = CheckIfShouldActivate();
    }


    protected virtual void ResolveCollisionEnter(Node node)
    {

    }

    protected virtual void ResolveCollisionExit(Node node)
    {

    }

    public bool CheckIfShouldActivate()
    {
        foreach (var akey in keys)
        {
            if (akey is not Entity.Key key) continue;

            if (!key.completed) return false;
        }

        GD.Print("Checked");
        return true;
    }

    protected void ResetKeys()
    {
        foreach (var akey in keys)
        {
            if (akey is not Entity.Key key) continue;

            key.completed = false;
        }
    }
}
