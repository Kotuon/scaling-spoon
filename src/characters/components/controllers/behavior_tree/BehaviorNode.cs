using System;
using Game.Component;
using Godot;
using Godot.Collections;

public partial class BehaviorNode : Node
{
    public enum Status
    {
        RUNNING,
        SUCCESS,
        ERROR,
    }

    public virtual Status evaluate(Dictionary context)
    {
        GD.PushError("Not implemented evaluate in behavior tree, " + Name);
        return Status.ERROR;
    }
}
