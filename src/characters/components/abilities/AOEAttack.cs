namespace Game.Component;

using Godot;

public partial class AOEAttack : Ability
{
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
}
