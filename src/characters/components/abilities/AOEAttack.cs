namespace Game.Component;

using Godot;

public partial class AOEAttack : Ability
{
    [Export] public PackedScene proj;
    [Export] float damage = 0.0f;
    private Area2D _area;
    protected Area2D area
    {
        private set => _area = value;
        get
        {
            if (_area == null)
                _area = GetNode<Area2D>("AOEArea");
            return _area;
        }
    }

    Tween tween;

    public AOEAttack() : base("AOEAttack")
    {

    }

    public override void _Ready()
    {
        base._Ready();

        tween = GetTree().CreateTween().SetLoops();
        tween.LoopFinished += (long _loop_count) => {
            SetShaderRunning(false);
            tween.Stop();
        };

        tween.TweenMethod(new Callable(this, MethodName.SetShaderParameter),
            0.0f, 1.0f, 1.0f);
        tween.Stop();

        SetShaderRunning(false);
    }


    public override void Trigger()
    {
        base.Trigger();

        if (!isActive) return;


    }

    public void DealDamageInArea()
    {
        SetShaderRunning(true);
        tween.Stop();
        tween.Play();

        var overlapping = area.GetOverlappingBodies();
        foreach (Node2D body in overlapping)
        {
            if (body is not IDamageable) continue;

            (body as IDamageable).Damage(damage);
        }
    }

    public void SetShaderRunning(bool value)
    {
        MeshInstance2D mesh = GetNode<MeshInstance2D>("MeshInstance2D");
        ShaderMaterial shader = mesh.Material as ShaderMaterial;
        shader.SetShaderParameter("is_running", value);
    }

    public void SetShaderParameter(float value)
    {
        MeshInstance2D mesh = GetNode<MeshInstance2D>("MeshInstance2D");
        ShaderMaterial shader = mesh.Material as ShaderMaterial;
        shader.SetShaderParameter("input_value", value);
    }

    public void LaunchProjectiles(int num_to_launch)
    {
        float dir = (float)GD.RandRange(0.0, 2.0 * (double)Mathf.Pi);
        float str = 60.0f;

        for (int i = 0; i < num_to_launch; ++i)
        {
            var inst = proj.Instantiate<Projectile>();
            parent.GetParent().AddChild(inst);

            Vector2 start_pos = parent.Position + Vector2.FromAngle(dir) * str;

            inst.Position = start_pos;
            inst.Rotation = dir;
            inst.launchDir = Vector2.FromAngle(dir);
            inst.owner = parent;

            inst.damage = damage;

            dir += 2 * Mathf.Pi / num_to_launch;
        }
    }
}
