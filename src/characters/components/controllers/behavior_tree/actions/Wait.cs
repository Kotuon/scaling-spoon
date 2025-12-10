using Godot;
using Godot.Collections;
using System;
using System.Runtime.CompilerServices;

public partial class Wait : BehaviorNode
{
    [Export] public double wait_time = 1.0;
    private double counter = 0.0;

    private bool is_active = false;

    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (!is_active)
            initialize();
        else if (counter >= wait_time)
            return completed();

        return BehaviorNode.Status.RUNNING;
    }

    private void initialize()
    {
        counter = 0.0f;
        is_active = true;
    }

    private BehaviorNode.Status completed()
    {
        is_active = false;
        return BehaviorNode.Status.SUCCESS;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (is_active)
            counter += delta;
    }

}
