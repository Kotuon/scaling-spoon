using Game.Component;
using Godot;
using Godot.Collections;
using System;

public partial class BehaviorNode : Node
{
    public enum Status
    {
        RUNNING,
        SUCCESS,
        ERROR
    }

    virtual public Status evaluate(Dictionary context)
    {
        GD.PushError("Not implemented evaluate in behavior tree, " + Name);
        return Status.ERROR;
    }
}
