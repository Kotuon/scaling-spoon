namespace Game.Entity;

using Godot;

public partial class LevelChange : Entity.Key, IInteractable
{
    [Export]
    protected string new_level = null;

    private CharacterBase playerRef;
    private bool playerInArea = false;

    private Control _control;
    protected Control Prompt
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
    public AnimatedSprite2D Anim
    {
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
        Anim.Play("loop");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GetTree().ChangeSceneToPacked(new_level);
    }

    public void Interact(CharacterBase @base)
    {
        if (@base is not Player)
            return;

        if (new_level == null)
            return;

        Global.Instance.GotoScene(new_level);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!playerInArea)
            return;

        if (@event.IsActionPressed("interact"))
        {
            Interact(playerRef);
        }
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        GD.Print(node.Name);
        if (node is not Player)
            return;

        playerRef = node as CharacterBase;
        playerInArea = true;

        if (!completed)
            Prompt.Visible = true;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (node is not Player)
            return;

        playerRef = null;
        playerInArea = false;

        if (!completed)
            Prompt.Visible = false;
    }
}
