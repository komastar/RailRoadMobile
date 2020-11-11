using UnityEngine;

public struct GridInt
{
    public int x;
    public int y;

    public GridInt(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public GridInt(Vector2Int xy)
    {
        x = xy.x;
        y = xy.y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}
