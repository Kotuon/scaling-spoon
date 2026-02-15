namespace Game.Entity;

using Godot;

public partial class ManaDrain : AreaTriggerItem
{
    [Export] float drain_amount = 10.0f;
    private bool inside = false;
    private Player playerRef = null;

    private bool should_update = false;
    [Export] private float time_between_drain = 1.0f;
    private float _counter;
    private float counter
    {
        set
        {
            _counter = value;
            if (_counter <= 0.0f)
            {
                should_update = true;
                _counter = time_between_drain;
            }
        }
        get => _counter;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!inside) return;

        Mana manaManager = playerRef.GetComponent<Mana>();
        manaManager.UseMana(10.0f * (float)delta);

        // counter -= (float)delta;

        // if (!should_update) return;
        // should_update = false;

        // manaManager.UseMana(10.0f);
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        if (node is not Player) return;

        inside = true;
        playerRef = node as Player;
        counter = 0.0f;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (node is not Player) return;

        inside = false;
        playerRef = null;
        should_update = false;
    }
}
