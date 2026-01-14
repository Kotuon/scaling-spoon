using Game.Component;
using Game.Entity;

using Godot;
using System;

public partial class MainUI : Control
{


    private CharacterBase _parent;
    public CharacterBase parent
    {
        protected set => _parent = value;

        get
        {
            if (_parent == null)
                _parent = GetNode<CharacterBase>("../..");
            return _parent;
        }
    }

    // private Camera2D _cameraRef;
    // public Camera2D cameraRef
    // {
    //     private set => _cameraRef = value;

    //     get
    //     {
    //         if (_cameraRef == null)
    //             _cameraRef = parent.GetComponent<Camera2D>();
    //         return _cameraRef;
    //     }
    // }

    private ProgressBar _manaBar;
    public ProgressBar manaBar
    {
        private set => _manaBar = value;

        get
        {
            if (_manaBar == null)
                _manaBar = GetNode<ProgressBar>("ManaBar");
            return _manaBar;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        parent.GetComponent<ManaManager>().mana_changed += UpdateManaBar;
    }

    
    public override void _Process(double delta)
    {
        base._Process(delta);

        // Position = cameraRef.Position + cameraRef.Offset;
    }

    private void UpdateManaBar(float newValue)
    {
        manaBar.Value = newValue;
    }
}
