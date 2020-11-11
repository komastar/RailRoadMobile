﻿using Assets.Constant;
using Assets.Object;
using UnityEngine;

public struct GridInt
{
    public GridInt(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
}

public struct MapModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float NodeSize { get; set; }
    public GridInt MapSize { get; set; }
    public NodeModel[] Nodes { get; set; }
}

public struct NodeModel
{
    public int Id { get; set; }
    public GridInt Position { get; set; }
    public Direction Direction { get; set; }

    public void Convert(NodeObject node)
    {
        Id = node.routeId;
        Position = node.Position;
        Direction = (Direction)node.direction;
    }
}