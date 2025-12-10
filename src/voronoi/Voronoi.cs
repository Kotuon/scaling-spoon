namespace Voronoi;

using Godot;
using System;
using System.Collections.Generic;

using Verticies = System.Collections.Generic.LinkedList<Godot.Vector2>;
using Edges = System.Collections.Generic.LinkedList<Edge>;

public partial class Voronoi : Node2D
{
    private Verticies places;
    private Edges edges = new Edges();
    private float width;
    private float height;
    private Parabola root = null;
    private float ly;

    private Verticies ver = new Verticies();
    private Verticies dir = new Verticies();

    private HashSet<Event> deleted = new HashSet<Event>();
    private LinkedList<Vector2> points = new LinkedList<Vector2>();
    private PriorityQueue<Event, float> queue = new PriorityQueue<Event, float>();
    private RandomNumberGenerator rng = new RandomNumberGenerator();

    private float _scale = 1000.0f;
    [Export]
    public float scale
    {
        get { return _scale; }
        set { _scale = value; QueueRedraw(); }
    }

    public Voronoi()
    {
        edges = null;
        rng.Randomize();
    }

    public override void _Ready()
    {
        base._Ready();

        for (int i = 0; i < 10; ++i)
        {
            ver.AddLast(new Godot.Vector2(rng.Randf() * scale, rng.Randf() * scale));
            dir.AddLast(new Godot.Vector2(
                rng.Randf() - 0.5f, rng.Randf() - 0.05f));
        }

        edges = GetEdges((int)scale, (int)scale);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _Draw()
    {
        base._Draw();

        var j = dir.First;
        for (var i = ver.First; i != null; i = i.Next)
        {
            var iVal = i.ValueRef;
            var jVal = j.ValueRef;

            iVal.X += jVal.X * scale / 50;
            iVal.Y += jVal.Y * scale / 50;

            if (iVal.X > scale)
                jVal.X *= -1;
            if (iVal.X < 0)
                jVal.X *= -1;

            if (iVal.Y > scale)
                jVal.Y *= -1;
            if (iVal.Y < 0)
                jVal.Y *= -1;

            j = j.Next;
        }

        edges = GetEdges((int)scale, (int)scale);

        Color[] colors = { Colors.Red };

        float offset = 10.0f;
        foreach (var i in ver)
        {
            Vector2[] points =
            {
                //    new Vector2(-1 + 2 * i.X / scale - 0.01f, -1 + 2 * i.Y / scale - 0.01f),
                //    new Vector2(-1 + 2 * i.X / scale + 0.01f, -1 + 2 * i.Y / scale - 0.01f),
                //    new Vector2(-1 + 2 * i.X / scale + 0.01f, -1 + 2 * i.Y / scale + 0.01f),
                //    new Vector2(-1 + 2 * i.X / scale - 0.01f, -1 + 2 * i.Y / scale + 0.01f),
                
                new Vector2(i.X - offset, i.Y - offset),
                new Vector2(i.X + offset, i.Y - offset),
                new Vector2(i.X + offset, i.Y + offset),
                new Vector2(i.X - offset, i.Y + offset),
            };

            foreach (var point in points)
            {
                GD.Print(point.X.ToString() + ", " + point.Y.ToString());
            }
            GD.Print("----------------------");

            DrawPolygon(
                points, colors
            );
        }

        foreach (var edge in edges)
        {
            // var start = new Vector2(-1 + 2 * edge.start.X / scale,
            //     -1 + 2 * edge.start.Y / scale);
            // var end = new Vector2(-1 + 2 * edge.end.X / scale,
            //     -1 + 2 * edge.end.Y / scale);

            var start = new Vector2(edge.start.X, edge.start.Y);
            var end = new Vector2(edge.end.X, edge.end.Y);

            DrawLine(start, end, Colors.Blue);
            // DrawLine(edge.start, edge.end, Colors.Blue);
        }

        foreach (var vert in ver)
        {
            DrawCircle(vert, 10.0f, Colors.Purple);
        }
    }

    public Edges GetEdges(int w, int h)
    {
        places = ver;
        width = w;
        height = h;
        root = null;

        points.Clear();
        edges.Clear();

        foreach (Godot.Vector2 vec in places)
        {
            queue.Enqueue(new Event(vec, true), vec.Y);
        }

        Event e;
        while (queue.Count > 0)
        {
            e = queue.Dequeue();

            ly = e.point.Y;
            if (deleted.Contains(e))
            {
                deleted.Remove(e);
                continue;
            }

            if (e.pe)
                InsertParabola(e.point);
            else
                RemoveParabola(ref e);
        }

        FinishEdge(root);

        foreach (Edge edge in edges)
        {
            if (edge.neighbour != null)
                edge.start = edge.neighbour.end;
        }

        return edges;
    }

    private void InsertParabola(Godot.Vector2 p)
    {
        if (root == null)
        {
            root = new Parabola(p);
            return;
        }

        if (root.isLeaf && root.site.Y - p.Y < 1.0f)
        {
            var fp = root.site;

            root.isLeaf = false;
            root.left = new Parabola(fp);
            root.right = new Parabola(p);

            Godot.Vector2 s = new Godot.Vector2((p.X + fp.X) / 2, height);
            points.AddLast(s);

            if (p.X > fp.X)
                root.edge = new Edge(s, fp, p);
            else
                root.edge = new Edge(s, p, fp);

            if (edges == null)
                edges = new Edges();

            edges.AddLast(root.edge);
            return;
        }

        var par = GetParabolaByX(p.X);

        if (par.cEvent != null)
        {
            deleted.Add(par.cEvent);
            par.cEvent = null;
        }

        Godot.Vector2 start = new Godot.Vector2(p.X, GetY(par.site, p.X));
        points.AddLast(start);

        var el = new Edge(start, par.site, p);
        var er = new Edge(start, p, par.site);

        el.neighbour = er;
        edges.AddLast(el);

        par.edge = er;
        par.isLeaf = false;

        var p0 = new Parabola(par.site);
        var p1 = new Parabola(p);
        var p2 = new Parabola(par.site);

        par.right = p2;
        par.left = new Parabola();
        par.left.edge = el;

        par.left.left = p0;
        par.left.right = p1;

        CheckCircle(ref p0);
        CheckCircle(ref p2);
    }

    private void RemoveParabola(ref Event e)
    {
        var p1 = e.arch;

        var xl = p1.GetLeftParent();
        var xr = p1.GetRightParent();

        var p0 = xl.GetLeftChild();
        var p2 = xr.GetRightChild();

        if (p0 == p2)
            GD.Print("Bad stuff");

        if (p0.cEvent != null)
        {
            deleted.Add(p0.cEvent);
            p0.cEvent = null;
        }

        if (p2.cEvent != null)
        {
            deleted.Add(p2.cEvent);
            p2.cEvent = null;
        }

        var p = new Godot.Vector2(e.point.X, GetY(p1.site, e.point.X));
        points.AddLast(p);

        xl.edge.end = p;
        xr.edge.end = p;

        Parabola higher = null;
        var par = p1;

        while (par != root)
        {
            par = par.parent;
            if (par == xl)
                higher = xl;
            if (par == xr)
                higher = xr;
        }

        higher.edge = new Edge(p, p0.site, p2.site);
        edges.AddLast(higher.edge);

        var gparent = p1.parent.parent;
        if (p1.parent.left == p1)
        {
            if (gparent.left == p1.parent)
                gparent.left = p1.parent.right;
            if (gparent.right == p1.parent)
                gparent.right = p1.parent.right;
        }
        else
        {
            if (gparent.left == p1.parent)
                gparent.left = p1.parent.left;
            if (gparent.right == p1.parent)
                gparent.right = p1.parent.left;
        }

        CheckCircle(ref p0);
        CheckCircle(ref p2);
    }

    private void FinishEdge(Parabola n)
    {
        if (n.isLeaf)
            return;

        float mx;

        if (n.edge.direction.X > 0.0f)
            mx = Mathf.Max(width, n.edge.start.X + 10.0f);
        else
            mx = Mathf.Min(0.0f, n.edge.start.X - 10.0f);

        var end = new Godot.Vector2(mx, mx * n.edge.f + n.edge.g);
        n.edge.end = end;
        points.AddLast(end);

        FinishEdge(n.left);
        FinishEdge(n.right);
    }

    private float GetXOfEdge(ref Parabola par, float y)
    {
        var left = par.GetLeftChild();
        var right = par.GetRightChild();

        var p = left.site;
        var r = right.site;

        float dp = 2.0f * (p.Y - y);
        float a1 = 1.0f / dp;
        float b1 = -2.0f * p.X / dp;
        float c1 = y + dp / 4.0f + p.X * p.X / dp;

        dp = 2.0f * (r.Y - y);

        float a2 = 1.0f / dp;
        float b2 = -2.0f * r.X / dp;
        float c2 = ly + dp / 4.0f + r.X * r.X / dp;

        float a = a1 - a2;
        float b = b1 - b2;
        float c = c1 - c2;

        float disc = b * b - 4.0f * a * c;
        float x1 = (-b + Mathf.Sqrt(disc)) / (2.0f * a);
        float x2 = (-b - Mathf.Sqrt(disc)) / (2.0f * a);

        float ry;
        if (p.Y < r.Y)
            ry = Mathf.Max(x1, x2);
        else
            ry = Mathf.Min(x1, x2);

        return ry;
    }

    private Parabola GetParabolaByX(float xx)
    {
        var par = root;
        float x = 0.0f;

        while (!par.isLeaf)
        {
            x = GetXOfEdge(ref par, ly);
            if (x > xx)
                par = par.left;
            else
                par = par.right;
        }

        return par;
    }

    private float GetY(Godot.Vector2 p, float x)
    {
        float dp = 2.0f * (p.Y - ly);
        float a1 = 1.0f / dp;
        float b1 = -2.0f * p.X / dp;
        float c1 = ly + dp / 4.0f + p.X * p.X / dp;

        return (a1 * x * x + b1 * x + c1);
    }

    private void CheckCircle(ref Parabola b)
    {
        var lp = b.GetLeftParent();
        var rp = b.GetRightParent();

        var a = (lp == null) ? null : lp.GetLeftChild();
        var c = (rp == null) ? null : rp.GetRightChild();

        if (a == null || c == null || a.site == c.site)
            return;

        var s = Godot.Vector2.Inf;
        s = GetEdgeIntersection(lp.edge, rp.edge);
        if (s == Godot.Vector2.Inf)
            return;

        float dx = a.site.X - s.X;
        float dy = a.site.Y - s.Y;

        float d = Mathf.Sqrt((dx * dx) + (dy * dy));

        var e = new Event(new Godot.Vector2(s.X, s.Y - d), false);
        points.AddLast(e.point);
        b.cEvent = e;
        e.arch = b;
        queue.Enqueue(e, e.point.Y);
    }

    private Godot.Vector2 GetEdgeIntersection(Edge a, Edge b)
    {
        float x = (b.g - a.g) / (a.f - b.f);
        float y = a.f * x + a.g;

        if ((x - a.start.X) / a.direction.X < 0.0f)
            return Godot.Vector2.Inf;
        if ((y - a.start.Y) / a.direction.Y < 0.0f)
            return Godot.Vector2.Inf;

        if ((x - b.start.X) / b.direction.X < 0.0f)
            return Godot.Vector2.Inf;
        if ((y - b.start.Y) / b.direction.Y < 0.0f)
            return Godot.Vector2.Inf;

        var p = new Godot.Vector2(x, y);
        points.AddLast(p);
        return p;
    }
}
