using Godot;

[Tool]
public partial class ChunkBase : Area2D
{
    private
      Vector2I id;

    private float tile_size = 2048;
    private StringName lastName;

    // Called when the node enters the scene tree for the first time.
    public
      override void _Ready()
    {
        lastName = Name;
        UpdatePositionFromId();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public
      override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            if (GetOwner() == null)
            {
                Position = Vector2.Zero;
                lastName = "";
            }
            else if (lastName != Name)
            {
                GD.Print(GetOwner().Name);
                UpdatePositionFromId();
                lastName = Name;

            }
        }
    }

    protected
      void UpdatePositionFromId()
    {
        var name = Name.ToString();

        var comma_loc = name.Find(",");

        if (comma_loc == -1)
        {
            GD.PushError("Tile name not grid value.");
        }

        var x = name.Substr(0, comma_loc);
        var y = name.Substr(comma_loc + 1, name.Length - comma_loc + 1);

        id.X = x.ToInt();
        id.Y = y.ToInt();

        Position =
            new Vector2((float)id.X * tile_size, -(float)id.Y * tile_size);
    }
}
