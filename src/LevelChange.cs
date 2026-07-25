namespace Game.Entity;

using Godot;

public partial class LevelChange : Entity.Key, IInteractable
{
    [Export]
    protected string new_level = null;

    [Export]
    protected bool single_use = false;

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

        var can_level_change = Global.Instance.CanLevelChange;
        if (can_level_change.ContainsKey(new_level))
        {
            if (can_level_change[new_level] == false)
            {
                return;
            }
        }
        else
        {
            can_level_change.Add(new_level, !single_use);
        }

        var levels = Global.Instance.Levels;
        if (!levels.ContainsKey(new_level))
        {
            while (
                ResourceLoader.LoadThreadedGetStatus(new_level)
                != ResourceLoader.ThreadLoadStatus.Loaded
            )
            {
                levels.Add(
                    new_level,
                    (PackedScene)ResourceLoader.LoadThreadedGet(new_level)
                );
            }
        }

        Global.Instance.GotoScene(new_level, @base);
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
        if (node is not Player)
            return;

        var can_level_change = Global.Instance.CanLevelChange;
        if (
            can_level_change.ContainsKey(new_level)
            && can_level_change[new_level] == false
        )
        {
            return;
        }

        var levels = Global.Instance.Levels;
        if (!levels.ContainsKey(new_level))
        {
            // levels.Add(new_level, GD.Load<PackedScene>(new_level));

            ResourceLoader.LoadThreadedRequest(new_level);
        }

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
