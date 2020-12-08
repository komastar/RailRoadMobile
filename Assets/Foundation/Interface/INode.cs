using UnityEngine;

public interface INode : IGameActor
{
    Vector2Int Position { get; set; }
    EDirection Direction { get; set; }
    ENodeType NodeType { get; set; }
    string Floor { get; set; }
}