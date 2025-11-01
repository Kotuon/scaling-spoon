namespace Voronoi;

public class Parabola
{
    public bool isLeaf { get; set; }
    public Godot.Vector2 site { get; set; }
    public Edge edge { get; set; } = null;
    public Event cEvent { get; set; } = null;
    public Parabola parent { get; set; } = null;

    private Parabola _left;
    public Parabola left
    {
        get { return _left; }
        set { _left = value; _left.parent = this; }
    }
    private Parabola _right;
    public Parabola right
    {
        get { return _right; }
        set { _right = value; _right.parent = this; }
    }

    public Parabola()
    {
        isLeaf = false;
    }

    public Parabola(Godot.Vector2 s)
    {
        site = s;
        isLeaf = true;
    }

    public Parabola GetLeftParent()
    {
        var par = parent;
        var pLast = this;

        while (par.left == pLast)
        {
            if (par.parent == null)
                return null;
            pLast = par;
            par = par.parent;
        }

        return par;
    }

    public Parabola GetRightParent()
    {
        var par = parent;
        var pLast = this;

        while (par.right == pLast)
        {
            if (par.parent == null)
                return null;
            pLast = par;
            par = par.parent;
        }

        return par;
    }

    public Parabola GetLeftChild()
    {
        if (this == null)
            return null;

        var par = left;

        while (!par.isLeaf)
            par = par.right;

        return par;
    }

    public Parabola GetRightChild()
    {
        if (this == null)
            return null;

        var par = left;

        while (!par.isLeaf)
            par = par.left;

        return par;
    }
}