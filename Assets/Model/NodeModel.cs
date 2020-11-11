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
