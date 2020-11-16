using UnityEngine;

public class NodeModel
{
    public int Id;
    public Vector2Int Position;
    public EDirection Direction;
    public ENodeType NodeType;
    public EJointType[] Joints;

    public void Convert(NodeObject node)
    {
        Id = node.Id;
        Position = node.Position;
        Direction = (EDirection)node.direction;
        NodeType = node.NodeType;
    }
}
