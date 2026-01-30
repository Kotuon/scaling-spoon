using Godot;
using System;

public partial class AreaTriggerItem : Area2D
{
    public override void _Ready()
    {
        base._Ready();

        BodyEntered += ResolveCollisionEnter;
        BodyExited += ResolveCollisionExit;
    }

    protected virtual void ResolveCollisionEnter(Node node)
    {
        
    }

    protected virtual void ResolveCollisionExit(Node node)
    {
        
    }

}
