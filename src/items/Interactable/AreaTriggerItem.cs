using Godot;
using System;

public partial class AreaTriggerItem : Area2D
{
    [Signal] public delegate void collisionEnterEventHandler(Node node);
    [Signal] public delegate void collisionExitEventHandler(Node node);
    public override void _Ready()
    {
        base._Ready();

        BodyEntered += ResolveCollisionEnter;
        BodyExited += ResolveCollisionExit;
    }

    protected virtual void ResolveCollisionEnter(Node node)
    {
        EmitSignal(SignalName.collisionEnter, node);
    }

    protected virtual void ResolveCollisionExit(Node node)
    {
        EmitSignal(SignalName.collisionExit, node);
    }

}
