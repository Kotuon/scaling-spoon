using Game.Component;

using Godot;
using System;

public partial class Health : Component {
    [Signal]
    public delegate void health_changedEventHandler( float newValue );
    [Export]
    public float max = 100.0f;

    private float _curr;
    public float curr {
        get => _curr;

        set {
            _curr = Mathf.Clamp( value, 0.0f, max );
            EmitSignal( SignalName.health_changed, _curr );
        }
    }

    public bool block = false;

    public override void _Ready() {
        base._Ready();
        curr = max;

        Shield shield = parent.GetComponent< Shield >();
        if ( shield != null ) {
            shield.startShield += () => { block = true; };
            shield.endShield += () => { block = false; };
        }
    }

    public bool Use( float cost ) {
        if ( Mathf.IsZeroApprox( cost ) || block ) return false;

        if ( curr - cost <= 0.0f ) {
            curr -= cost;
            return true;
        }

        curr -= cost;
        return false;
    }

    public void Restore( float amount ) {
        if ( curr + amount >= max )
            curr = max;
        else
            curr += amount;
    }
}
