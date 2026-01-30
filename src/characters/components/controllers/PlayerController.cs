namespace Game.Component;

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;


enum InputState : int
{
    None,
    Pressed,
    Released,
}

public partial class PlayerController : Controller
{
    public override Vector2 moveInput
    {
        set => _moveInput = value;
        get
        {
            _moveInput = Input.GetVector("walk_left",
                "walk_right", "walk_up", "walk_down");

            return _moveInput;
        }
    }


[Export]
    public Godot.Collections.Dictionary<string, Ability> actionMap = 
        new Godot.Collections.Dictionary<string, Ability>();
        
    public override void _Ready()
    {
        base._Ready();

        var actions = GetNode<AbilityManager>("../AbilityManager").GetChildren();

        foreach (Node action in actions)
        {
            Ability ability = action as Ability;
            actionMap.Add(ability.abilityName, ability);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        foreach (var action in actionMap)
        {
            Ability ability = action.Value;
            
            if (@event.IsActionPressed(action.Key))
            {
                if (!ability.isActive && !ability.onCooldown)
                {
                    ability.Trigger();
                }
            }
            if (@event.IsActionReleased(action.Key))
            {
                if (ability.isActive)
                {
                    ability.StartCooldown();
                    ability.End();
                }
            }
        }
    }

}
