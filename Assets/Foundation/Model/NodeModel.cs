using Newtonsoft.Json;
using UnityEngine;

public class NodeModel
{
    public int Id;
    public int RouteId;
    public Vector2Int Position;
    public EDirection Direction;
    public ENodeType NodeType;
    [JsonIgnore]
    public EJointType[] Joints;
    public string Floor;

    public void Convert(INode node)
    {
        Id = node.Id;
        RouteId = node.RouteId;
        Position = node.Position;
        Direction = node.Direction;
        NodeType = node.NodeType;
        Floor = node.Floor;
    }
}
