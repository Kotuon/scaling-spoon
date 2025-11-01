namespace Voronoi;

public class Edge
{
    public Godot.Vector2 start { get; set; }
    public Godot.Vector2 end { get; set; }
    public Godot.Vector2 direction { get; set; }
    public Godot.Vector2 left { get; set; }
    public Godot.Vector2 right { get; set; }
    public float f { get; set; }
    public float g { get; set; }
    public Edge neighbour { get; set; } = null;

    public Edge(Godot.Vector2 s, Godot.Vector2 a, Godot.Vector2 b)
    {
        start = s;
        left = a;
        right = b;

        f = (b.X - a.X) / (a.Y - b.Y);
        g = s.Y - f * s.X;
        direction = new Godot.Vector2(b.Y - a.Y, -(b.X - a.X));
    }
}