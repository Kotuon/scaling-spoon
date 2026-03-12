namespace Game.Math;

using Godot;

public static class Curves
{
    public static float QuadLerp(float a, float b, float c, float t)
    {
        float p0 = Mathf.Lerp(a, b, t);
        float p1 = Mathf.Lerp(b, c, t);
        float p2 = Mathf.Lerp(p0, p1, t);

        return p2;
    }
    public static float CubicLerp(float a, float b, float c, float d, float t)
    {
        float p0 = Mathf.Lerp(a, b, t);
        float p1 = Mathf.Lerp(b, c, t);
        float p2 = Mathf.Lerp(c, d, t);

        float p3 = Mathf.Lerp(p0, p1, t);
        float p4 = Mathf.Lerp(p1, p2, t);

        float p5 = Mathf.Lerp(p3, p4, t);

        return p5;
    }
    public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2,
        float t)
    {
        Godot.Vector2 q0 = p0.Lerp(p1, t);
        Godot.Vector2 q1 = p1.Lerp(p2, t);

        return q0.Lerp(q1, t);
    }
    public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2,
        Vector2 p3, float t)
    {
        Vector2 q0 = p0.Lerp(p1, t);
        Vector2 q1 = p1.Lerp(p2, t);
        Vector2 q2 = p2.Lerp(p3, t);

        Vector2 r0 = q0.Lerp(q1, t);
        Vector2 r1 = q1.Lerp(q2, t);

        Vector2 s = r0.Lerp(r1, t);
        return s;
    }
};


