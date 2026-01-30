namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class AnimationHandler : Component
{
    [Export(PropertyHint.Enum, "CrimsonRed, ")]
    public string animationLibrary = "CrimsonRed";
    [ExportGroup("Speed Cutoffs")]
    [Export] public float runCutoff { get; set; } = 200.0f;
    [Export] public float dashCutoff { get; set; } = 400.0f;

    private AnimationPlayer _animationPlayer;
    public AnimationPlayer animationPlayer
    {
        protected set => _animationPlayer = value;

        get
        {
            if (_animationPlayer == null)
                _animationPlayer = parent.GetComponent<AnimationPlayer>();
            return _animationPlayer;
        }
    }
    private Sprite2D _sprite;
    public Sprite2D sprite
    {
        protected set => _sprite = value;

        get
        {
            if (_sprite == null)
                _sprite = parent.GetComponent<Sprite2D>();
            return _sprite;
        }
    }

    private enum Direction
    {
        FRONT = 0,
        FRONT_S = 1,
        SIDE = 2,
        BACK_S = 3,
        BACK = 4
    }

    private Direction currDir = Direction.FRONT;
    [Export] public int currFrame = 0;

    private Vector2 lastNonZeroInput = new Vector2(0, 1);
    private float dirCutoff = 0.25f;
    public bool canAdvance = true;

    private bool _flip;
    public bool flip
    {
        set
        {
            _flip = value;
            sprite.FlipH = _flip;
            ShaderMaterial shader = sprite.Material as ShaderMaterial;

            if (shader == null)
                return;

            shader.SetShaderParameter("flip", _flip);
        }

        get => _flip;
    }

    public void set_frame(int newFrame)
    {
        currFrame = newFrame;
        sprite.FrameCoords = new Vector2I(currFrame, (int)currDir);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer.Play(animationLibrary + "/idle_default");
        sprite.TextureChanged += UpdateShaderForTexture;

        animationPlayer.GetAnimationLibraryList();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!canAdvance)
        {
            if (animationPlayer.CurrentAnimationPosition >=
                animationPlayer.CurrentAnimationLength)
            {
                canAdvance = true;
                parent.GetComponent<Move>().canMove = true;
                mouseRef.useMouseDirection = false;
            }
        }
        else if (parent.GetComponent<Move>().canMove &&
            !parent.GetComponent<Move>().movementOverride)
        {
            // Vector2 currVelocity = ClipSmallValues(parent.GetRealVelocity());
            Vector2 currVelocity = ClipSmallValues(parent.Velocity);
            UpdateWalkAnimation(currVelocity);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public String GetCurrentAnimation()
    {
        return animationPlayer.CurrentAnimation;
    }
    public void PlayAnimation(String animName, Vector2 inputDir)
    {
        currDir = GetAnimationDirection(inputDir);

        flip = inputDir.X > 0 ? false : true;

        if (Mathf.IsZeroApprox(inputDir.LengthSquared()))
        {
            currDir = GetAnimationDirection(lastNonZeroInput);
            flip = lastNonZeroInput.X > 0 ? false : true;
        }

        animationPlayer.Play(animationLibrary + "/" + animName);
    }

    private Direction GetAnimationDirection(Vector2 currVelocity)
    {
        Vector2 direction = currVelocity.Normalized();

        Direction animationDirection = Direction.FRONT;

        if (!Mathf.IsZeroApprox(currVelocity.LengthSquared()))
            lastNonZeroInput = currVelocity.Normalized();

        if (direction.Y > dirCutoff)
        {
            if (Mathf.Abs(direction.X) < 0.1f)
                animationDirection = Direction.FRONT;
            else
                animationDirection = Direction.FRONT_S;
        }
        else if (direction.Y < -dirCutoff)
        {
            if (Mathf.Abs(direction.X) < 0.1f)
                animationDirection = Direction.BACK;
            else
                animationDirection = Direction.BACK_S;
        }
        else
            animationDirection = Direction.SIDE;

        return animationDirection;
    }

    private void UpdateWalkAnimation(Vector2 currVelocity)
    {
        float speed = currVelocity.Length();

        var mouseDirection = mouseRef.GlobalPosition - parent.GlobalPosition;

        currDir = mouseRef.useMouseDirection
            ? GetAnimationDirection(mouseDirection)
            : GetAnimationDirection(currVelocity);

        if (Mathf.IsZeroApprox(speed))
        {
            currDir = GetAnimationDirection(lastNonZeroInput);
            animationPlayer.Play(animationLibrary + "/idle_default");
            flip = lastNonZeroInput.X < 0 ? true : false;
        }
        else if (speed > runCutoff)
        {
            animationPlayer.Play(animationLibrary + "/walk");

            flip = mouseRef.useMouseDirection ? mouseDirection.X < 0
                : currVelocity.X < 0;
        }
        else
        {
            animationPlayer.Play(animationLibrary + "/walk");

            flip = mouseRef.useMouseDirection ? mouseDirection.X < 0
                : currVelocity.X < 0;
        }

    }

    private Vector2 ClipSmallValues(Vector2 inputVec)
    {
        if (Mathf.Abs(inputVec.X) < 0.1f)
            inputVec.X = 0.0f;
        if (Mathf.Abs(inputVec.Y) < 0.1f)
            inputVec.Y = 0.0f;

        return inputVec;
    }

    private void UpdateShaderForTexture()
    {
        ShaderMaterial shader = sprite.Material as ShaderMaterial;

        if (shader == null)
            return;

        shader.SetShaderParameter("hframes", sprite.Hframes);
        shader.SetShaderParameter("vframes", sprite.Vframes);
    }
}
