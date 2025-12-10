namespace Voronoi;

public class Event
{
    public Godot.Vector2 point { get; set; }
    public bool pe { get; set; }
    public float y { get; set; }
    public Parabola arch { get; set; }

    public Event(Godot.Vector2 p, bool pev)
    {
        point = p;
        pe = pev;
        y = p.Y;
    }
}