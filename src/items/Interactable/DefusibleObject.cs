namespace Game.Entity;

using Godot;

public partial class DefusibleObject : Entity.Key, IInteractable
{
    [Export] public float startingMana = 100.0f;
    [Export] public float timeToInfuse = 1.0f;
    private float currMana = 0.0f;
    private CharacterBase playerRef;
    private bool playerInArea = false;
    private Control _control;
    protected Control control
    {
        private set => _control = value;

        get
        {
            _control ??= GetNode<Control>("Control");

            return _control;
        }
    }
    private Player player_ref;
    private AnimatedSprite2D _anim;
    public AnimatedSprite2D anim {
        private set => _anim = value;
        get
        {
            _anim ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            return _anim;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        player_ref = GetTree().GetNodesInGroup("Player")[0] as Player;
        anim.Play("loop");

        currMana = startingMana;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }


    public void Interact(CharacterBase @base)
    {
        if (currMana <= 0.0f) return;

        Mana mana = @base.GetComponent<Mana>();
        if (mana == null) return;

        mana.RestoreMana(currMana);
        Infuse();
    }

    private void Infuse()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "currMana", 0.0f, 1.7f);

        completed = true;
        control.Visible = false;

        anim.Play("out");
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
        GD.Print(node.Name);
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
