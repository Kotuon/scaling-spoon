using Godot;
using Godot.Collections;
using System;

public partial class RepeatUntilFail : DecoratorNode
{
    public override Status evaluate(Dictionary context)
    {
        BehaviorNode.Status status = m_child.evaluate(context);
        if (status == BehaviorNode.Status.ERROR)
            return BehaviorNode.Status.ERROR;
        
        return BehaviorNode.Status.RUNNING;
    }

}
