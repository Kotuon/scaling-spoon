namespace Game.Component;

using Game.Entity;
using Godot;

public partial class EnemyMouse : Mouse
{
    private CharacterBase _playerRef;
    protected CharacterBase playerRef
    {
        private set => _playerRef = value;
        get
        {
            if (_playerRef == null)
            {
                _playerRef =
                    GetTree().GetFirstNodeInGroup("Player") as CharacterBase;
            }
            return _playerRef;
        }
    }
    public override void _Process(double delta)
    {
        icon.Visible = true;

        Vector2 target = playerRef.GlobalPosition +
            (playerRef.Velocity);

        GlobalPosition =
            (target - parent.GlobalPosition).Normalized() *
            100.0f + parent.GlobalPosition;
    }

}
