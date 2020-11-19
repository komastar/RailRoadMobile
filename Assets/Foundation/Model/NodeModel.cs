using UnityEngine;

public class NodeModel
{
    public int Id;
    public Vector2Int Position;
    public EDirection Direction;
    public ENodeType NodeType;
    public EJointType[] Joints;

    public void Convert(INode node)
    {
        Id = node.Id;
        Position = node.Position;
        Direction = node.Direction;
        NodeType = node.NodeType;
    }
}
