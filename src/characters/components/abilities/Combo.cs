namespace Game.Component;

using Godot;
using Godot.Collections;
using Game.Entity;

public partial class Combo : Ability {
	[Export]
	public float damage = 5.0f;
	private int maxAttack = 2;
	private int currAttack = 0;
	private bool triggerNext = false;

	private Area2D _damageArea = null;
	protected Area2D damageArea { private
		set => _damageArea = value;
		get {
			if ( _damageArea == null ) {
				_damageArea = GetNode< Area2D >( "DamageArea" );
			}
			return _damageArea;
		}
	}

	private Dictionary< StringName, Node2D > bodiesInDamageArea = [];

	Combo() : base( "combo" ) {}

	public override void _Ready() {
		base._Ready();

		damageArea.BodyEntered += BodyEntered;
		damageArea.BodyExited += BodyExited;
	}

	public override void Pressed() {
		// base.Pressed();
		Trigger();
	}

	public override void Released() {
		// base.Released();
	}

	public override void Trigger() {
		base.Trigger();

		if ( !triggerNext ) currAttack += 1;

		// GD.Print(currAttack);

		if ( currAttack > maxAttack ) {
			triggerNext = false;
			return;
		}

		if ( currAttack == 1 ) {
			var inputDirection = parent.GetComponent< Controller >().moveInput;
			animHandler.PlayAnimation( abilityName + "_init", inputDirection );
		}

		move.canMove = false;
		triggerNext = true;
	}

	public override void _Process( double delta ) {

		var inputDirection = parent.GetComponent< Controller >().lastInput;
		UpdateHitbox( inputDirection );

		base._Process( delta );
	}

	public override void Update( double delta ) {
		base.Update( delta );

		if ( !isActive ) return;

		if ( animHandler.GetCurrentAnimation().Find( abilityName ) == -1 ) {
			if ( triggerNext ) {
				// var name = abilityName + "_" + currAttack.ToString();
				// GD.Print(name);

				var inputDirection =
					parent.GetComponent< Controller >().lastInput;
				animHandler.PlayAnimation(
					abilityName + "_" + currAttack.ToString(), inputDirection );

				UpdateHitbox( inputDirection );

				triggerNext = false;
			} else {
				StartCooldown();
				End();
			}
		}
	}

	public void TriggerHit() {
		foreach ( var ( _name, body ) in bodiesInDamageArea ) {
			( body as IDamageable ).Damage( damage );
		}
	}

	public override void End() {
		base.End();

		// GD.Print("End");

		currAttack = 0;
		triggerNext = false;

		var inputDirection = parent.GetComponent< Controller >().moveInput;
		animHandler.PlayAnimation( abilityName + "_end", inputDirection );
		animHandler.canAdvance = false;
	}

	private void UpdateHitbox( Vector2 input ) {
		// move attack hitbox based on input direction
		float angle = input.Angle();

		float length = Position.Length();

		var newPosition = Vector2.FromAngle( angle ) * length;
		Position = newPosition;
	}

	private void BodyEntered( Node2D body ) {
		GD.Print( body.Name );
		if ( body is IDamageable &&
			 !bodiesInDamageArea.ContainsKey( body.Name ) ) {
			bodiesInDamageArea.Add( body.Name, body );
		}
	}

	private void BodyExited( Node2D body ) {
		if ( bodiesInDamageArea.ContainsKey( body.Name ) ) {
			bodiesInDamageArea.Remove( body.Name );
		}
	}
}
