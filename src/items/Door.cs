namespace Game.Entity;

using Godot;

public partial class Door : AreaTriggerItem, IInteractable
{
    [Export] private Godot.Collections.Array<Key> keys =
        new Godot.Collections.Array<Key>();

    private bool active = false;

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

    public override void _Ready()
    {
        base._Ready();

        var children = GetChildren();

        foreach (var child in children)
        {
            if (child is not Key)
            {
                continue;
            }

            keys.Add(child as Key);
        }

        GD.Print(keys.Count);

        animPlayer.Play("Portal/idle_inactive");
        control.Visible = false;
    }

    public void Interact(CharacterBase @base)
    {
        if (!CheckForInfusedKeys()) return;

        animPlayer.AnimationSetNext("Portal/activate", "Portal/idle_active");
        animPlayer.Play("Portal/activate");

        active = true;
        control.Visible = false;
    }

    private bool CheckForInfusedKeys()
    {
        foreach (var key in keys)
        {
            if (!key.completed)
                return false;
        }

        return true;
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

        if (CheckForInfusedKeys() && !active)
            control.Visible = true;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (node is not Player) return;

        playerRef = null;
        playerInArea = false;

        control.Visible = false;
    }

}
