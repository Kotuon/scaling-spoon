namespace Game.Component;

using Game.Entity;
using Godot;

public partial class ObstacleComponent : Node2D, IAutoDoor
{
    protected Godot.Collections.Array<Entity.Key> keys =
        new Godot.Collections.Array<Entity.Key>();

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

        var children = GetChildren();

        foreach (var child in children)
        {
            if (child is not Entity.Key)
            {
                continue;
            }

            keys.Add(child as Entity.Key);
            GD.Print(Name + " Keys: " + keys.Count);
        }
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
        foreach (var key in keys)
        {
            if (!key.completed) return false;
        }

        GD.Print("Checked");
        return true;
    }
}
