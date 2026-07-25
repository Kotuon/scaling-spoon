using System.Diagnostics;
using Game.Entity;
using Godot;

public partial class FloatingItem : Area2D, IInteractable
{
    enum Direction
    {
        UP,
        DOWN,
    }

    private Direction CurrDirection = Direction.UP;

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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MoveUp();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }

    protected void MoveUp()
    {
        Tween tween = GetTree()
            .CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        var spear = GetNode<Node2D>("SpearSprite");

        tween.TweenProperty(
            spear,
            "position",
            spear.Position + new Vector2(0.0f, -10.0f),
            1.25f
        );
        tween.TweenCallback(Callable.From(MoveDown));
    }

    protected void MoveDown()
    {
        Tween tween = GetTree()
            .CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        var spear = GetNode<Node2D>("SpearSprite");

        tween.TweenProperty(
            spear,
            "position",
            spear.Position + new Vector2(0.0f, 10.0f),
            1.25f
        );
        tween.TweenCallback(Callable.From(MoveUp));
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

    public void Interact(CharacterBase @base) { }
}
