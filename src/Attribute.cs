namespace Game;

using Godot;
using System;

public partial class Attribute : GodotObject
{
    [Signal] public delegate void attribute_changedEventHandler(Variant v);
    [Export] private Variant max;
    [Export] private Variant min;
    [Export] private Variant initial;

    private Variant _curr;
    public Variant curr
    {
        get => _curr;
        
        set => _curr = value;
    }

    public void Init()
    {
        curr = initial;
    }
}
