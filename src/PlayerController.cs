namespace Game.Component;

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

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
        protected set => _moveInput = value;
        get
        {
            _moveInput = Input.GetVector("walk_left",
                "walk_right", "walk_up", "walk_down");

            return _moveInput;
        }
    }

    // Dictionary actionMap;
        
    public override void _Ready()
    {
        base._Ready();

        // var actions = InputMap.GetActions();

        // foreach (var action in actions)
        // {
        //     actionMap.Add(action, (int)InputState.None);
        // }
    }

    // public override void _Input(InputEvent @event)
    // {
    //     base._Input(@event);

    //     if (actionMap.ContainsKey(@event.AsText()))
    //     {
    //         if (@event.IsPressed())
    //             actionMap[@event.AsText()] = (int)InputState.Pressed;
    //         else if (@event.IsReleased())
    //             actionMap[@event.AsText()] = (int)InputState.Released;
    //     }
    // }


}
