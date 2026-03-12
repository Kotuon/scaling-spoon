namespace Game.Component;

using Godot;
using System;

public partial class AnimationHandler : Component
{
    protected enum Direction
    {
        FRONT = 0,
        FRONT_S = 1,
        SIDE = 2,
        BACK_S = 3,
        BACK = 4
    }

    [Export(PropertyHint.Enum, "CrimsonRed,golem")]
    public string animationLibrary = "CrimsonRed";
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

    protected Direction currDir = Direction.FRONT;
    protected float dirCutoff = 0.25f;

    protected Vector2 lastNonZeroInput = new Vector2(0, 1);

    [Export] public bool canAdvance = true;

    ////////////////////////////////////////////////////////////////////////////

    public override void _Process(double delta)
    {
        if (!canAdvance)
        {
            if (!IsCurrentlyPlaying())
            {
                canAdvance = true;
                parent.GetComponent<Move>().canMove = true;
                mouseRef.useMouseDirection = false;
            }
        }
        else if (parent.GetComponent<Move>().canMove &&
            !parent.GetComponent<Move>().movementOverride)
        {
            Vector2 currVelocity = ClipSmallValues(parent.Velocity);
            UpdateWalkAnimation(currVelocity);
        }
    }

    protected virtual void UpdateWalkAnimation(Vector2 currVelocity)
    {
        float speed = currVelocity.Length();

        currDir = GetAnimationDirection(currVelocity);

        if (Mathf.IsZeroApprox(speed))
        {
            currDir = GetAnimationDirection(lastNonZeroInput);
            animationPlayer.Play(animationLibrary + "/idle_default");
            flip = lastNonZeroInput.X < 0 ? true : false;
        }
        else
        {
            animationPlayer.Play(animationLibrary + "/walk");

            flip = currVelocity.X < 0;
        }
    }

    protected virtual Direction GetAnimationDirection(Vector2 currVelocity)
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

    public void PlayAnimation(String animName, Vector2 inputDir,
        float speed, bool fromEnd)
    {
        currDir = GetAnimationDirection(inputDir);

        flip = inputDir.X > 0 ? false : true;

        if (Mathf.IsZeroApprox(inputDir.LengthSquared()))
        {
            currDir = GetAnimationDirection(lastNonZeroInput);
            flip = lastNonZeroInput.X > 0 ? false : true;
        }

        animationPlayer.Play(animationLibrary + "/" + animName, -1, speed,
            fromEnd);
    }


    protected Vector2 ClipSmallValues(Vector2 inputVec)
    {
        if (Mathf.Abs(inputVec.X) < 0.1f)
            inputVec.X = 0.0f;
        if (Mathf.Abs(inputVec.Y) < 0.1f)
            inputVec.Y = 0.0f;

        return inputVec;
    }

    public bool IsCurrentlyPlaying()
    {
        return animationPlayer.CurrentAnimationPosition <
                animationPlayer.CurrentAnimationLength;
    }

    public bool IsActive()
    {
        return animationPlayer.IsAnimationActive();
    }

    public String GetCurrentAnimation()
    {
        return animationPlayer.CurrentAnimation;
    }
}
