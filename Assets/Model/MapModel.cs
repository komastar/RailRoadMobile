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

public class MapModel
{
    public int Id;
    public string Name;
    public float NodeSize;
    public GridInt MapSize;
    public NodeModel[] Nodes;

    public MapModel()
    {
        Name = "NewMap";
        NodeSize = 1f;
    }
}

public struct NodeModel
{
    public int Id;
    public GridInt Position;
    public Direction Direction;

    public void Convert(NodeObject node)
    {
        Id = node.routeId;
        Position = new GridInt(node.Position);
        Direction = (Direction)node.direction;
    }
}
