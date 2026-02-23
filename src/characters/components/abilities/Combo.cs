namespace Game.Component;

using Godot;
using System;

public partial class Combo : Ability
{
    private int maxAttack = 2;
    private int currAttack = 0;
    private bool triggerNext = false;
    Combo() : base("combo")
    {

    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void Pressed()
    {
        // base.Pressed();
        Trigger();
    }

    public override void Released()
    {
        // base.Released();
    }

    public override void Trigger()
    {
        base.Trigger();

        if (!triggerNext)
            currAttack += 1;

        GD.Print(currAttack);

        if (currAttack > maxAttack)
        {
            triggerNext = false;
            return;
        }

        if (currAttack == 1)
        {
            var inputDirection = parent.GetComponent<Controller>().moveInput;
            animHandler.PlayAnimation(abilityName + "_init", inputDirection);
        }

        move.canMove = false;
        triggerNext = true;
    }

    public override void Update(double delta)
    {
        base.Update(delta);

        if (!isActive) return;

        if (animHandler.GetCurrentAnimation().Find(abilityName) == -1)
        {
            if (triggerNext)
            {
                var name = abilityName + "_" + currAttack.ToString();
                GD.Print(name);

                var inputDirection =
                    parent.GetComponent<Controller>().moveInput;
                animHandler.PlayAnimation(
                    abilityName + "_" + currAttack.ToString(), inputDirection);

                triggerNext = false;
            }
            else
            {
                StartCooldown();
                End();
            }
        }
    }


    public override void End()
    {
        base.End();

        GD.Print("End");

        currAttack = 0;
        triggerNext = false;

        var inputDirection = parent.GetComponent<Controller>().moveInput;
        animHandler.PlayAnimation(abilityName + "_end", inputDirection);
        animHandler.canAdvance = false;
    }


    private void UpdateHitbox(Vector2 input)
    {
        // move attack hitbox based on input direction
    }
}
