using Godot;
using Godot.Collections;
using System;

public partial class Negator : DecoratorNode
{
    public override Status evaluate(Dictionary context)
    {
        Status status = m_child.evaluate(context);

        if (status == BehaviorNode.Status.ERROR)
            return BehaviorNode.Status.SUCCESS;
        else if (status == BehaviorNode.Status.SUCCESS)
            return BehaviorNode.Status.ERROR;

        return BehaviorNode.Status.RUNNING;
    }


}
