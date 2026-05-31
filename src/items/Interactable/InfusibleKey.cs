namespace Game.Entity;

using Godot;

public partial class InfusibleKey : Entity.Key, IInteractable
{
    [Export] public float neededMana = 10.0f;
    [Export] public float timeToInfuse = 1.0f;
    private float currMana = 0.0f;

    private CharacterBase playerRef;
    private bool playerInArea = false;

    private AnimationPlayer _animPlayer;
    public AnimationPlayer animPlayer
    {
        private set => _animPlayer = value;

        get
        {
            if (_animPlayer == null)
                _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            return _animPlayer;
        }
    }

    private Control _control;
    protected Control control
    {
        private set => _control = value;

        get
        {
            if (_control == null)
                _control = GetNode<Control>("Control");

            return _control;
        }
    }
    private Sprite2D pillar_spr;
    private Player player_ref;

    public override void _Ready()
    {
        base._Ready();

        control.Visible = false;

        pillar_spr = GetNode<Sprite2D>("Pillar");
        player_ref = GetTree().GetNodesInGroup("Player")[0] as Player;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        (pillar_spr.Material as ShaderMaterial).SetShaderParameter(
            "player_pos", player_ref.GetGlobalTransformWithCanvas().Origin);
    }


    public void Interact(CharacterBase @base)
    {
        if (currMana >= neededMana) return;

        Mana mana = @base.GetComponent<Mana>();
        if (mana == null) return;

        if (mana.UseMana(neededMana))
        {
            Infuse();
        }
    }

    private void Infuse()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "currMana", neededMana, 1.7f);

        completed = true;
        control.Visible = false;

        animPlayer.AnimationSetNext("Obelisk/activate", "Obelisk/idle");
        animPlayer.Play("Obelisk/activate");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!playerInArea) return;

        if (@event.IsActionPressed("interact"))
        {
            Interact(playerRef);
        }
    }


    protected override void ResolveCollisionEnter(Node node)
    {
        if (node is not Player) return;

        playerRef = node as CharacterBase;
        playerInArea = true;

        if (!completed) control.Visible = true;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (node is not Player) return;

        playerRef = null;
        playerInArea = false;

        if (!completed) control.Visible = false;
    }
}
