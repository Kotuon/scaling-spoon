using Godot;
using System;

public partial class Key : AreaTriggerItem
{
    public bool completed = false;

    public void Here()
    {
        GD.Print("HERE");
    }
}
