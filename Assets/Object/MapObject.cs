using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class MapObject : MonoBehaviour
{
    public NodeObject nodePrefab;

    private DataManager dataManager;
    private SpriteManager spriteManager;

    private Dictionary<Vector2Int, NodeObject> entireNodes;
    private HashSet<NodeObject> openNodes;
    private HashSet<NodeObject> openNodesBuffer;
    private HashSet<NodeObject> closedNodes;
    private HashSet<NodeObject> closedNodesBuffer;

    public void Init()
    {
        entireNodes = new Dictionary<Vector2Int, NodeObject>();
        openNodes = new HashSet<NodeObject>();
        closedNodes = new HashSet<NodeObject>();
        openNodesBuffer = new HashSet<NodeObject>();
        closedNodesBuffer = new HashSet<NodeObject>();

        dataManager = DataManager.Get();
        spriteManager = SpriteManager.Get();
    }

    public void MakeEmptyMap(MapModel mapData, RouteModel route)
    {
        Clear();
        Vector2Int mapSize = mapData.MapSize;
        float nodeSize = mapData.NodeSize;
        float offsetX = (nodeSize * 0.5f) * (mapData.MapSize.x - 1);
        float offsetY = (nodeSize * 0.5f) * (mapData.MapSize.y - 1);

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                var newNode = Instantiate(nodePrefab, transform);
                newNode.SetupNode(route, null);
                newNode.transform.localPosition = new Vector3(x * nodeSize - offsetX, y * nodeSize - offsetY, 0);
                newNode.transform.localScale = Vector3.one * nodeSize;
                newNode.Position = new Vector2Int(x, y);
            }
        }

        var orthoSize = Math.Max(mapSize.x * nodeSize, mapSize.y * nodeSize);
        Camera.main.orthographicSize = orthoSize;
    }

    public void MakeMap(MapModel mapData
        , Dictionary<int, RouteModel> routeData = null
        , Dictionary<string, Sprite> spriteData = null)
    {
        Clear();
        if (routeData == null)
        {
            routeData = DataManager.Get().RouteData;
        }

        if (spriteData == null)
        {
            spriteData = SpriteManager.Get().routeSprites;
        }

        Vector2Int mapSize = mapData.MapSize;
        float nodeSize = mapData.NodeSize;
        float offsetX = (nodeSize * 0.5f) * (mapData.MapSize.x - 1);
        float offsetY = (nodeSize * 0.5f) * (mapData.MapSize.y - 1);

        var nodes = mapData.Nodes;
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];
            var newNode = Instantiate(nodePrefab, transform);
            Sprite sprite = null;
            if (spriteData.ContainsKey(routeData[node.Id].Name))
            {
                sprite = spriteData[routeData[node.Id].Name];
            }

            newNode.SetupNode(node, routeData[node.Id], sprite);
            newNode.transform.localPosition = new Vector3(node.Position.x - offsetX, node.Position.y - offsetY, 0f);
            newNode.transform.localScale = Vector3.one * nodeSize;

            NewNode(newNode);
        }

        var orthoSize = Math.Max(mapSize.x * nodeSize, mapSize.y * nodeSize);
        Camera.main.orthographicSize = orthoSize;
    }

    public void Clear()
    {
        var children = GetComponentsInChildren<NodeObject>();
        for (int i = 0; i < children.Length; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }
    }

    public void OpenMap()
    {
        OpenEntrances();
        ExpandNodes();
    }

    private void OpenEntrances()
    {
        foreach (var node in entireNodes)
        {
            ENodeType nodeType = node.Value.NodeType;
            switch (nodeType)
            {
                case ENodeType.Entrance:
                    OpenNode(node.Value);
                    break;
                case ENodeType.None:
                case ENodeType.Normal:
                case ENodeType.Wall:
                    break;
            }
        }
    }

    private void NewNode(NodeObject node)
    {
        entireNodes.Add(node.Position, node);
    }

    private void OpenNode(NodeObject node)
    {
        openNodes.Add(node);
        node.Open();
    }

    private void CloseNode(NodeObject node)
    {
        openNodes.Remove(node);
        node.Close();
    }

    private void ExpandNodes()
    {
        foreach (var node in openNodes)
        {
            Debug.Log($"Expand : {node.name}");
            ExpandNode(node);
        }

        foreach (var node in openNodesBuffer)
        {
            Debug.Log($"Open : {node.name}");
            OpenNode(node);
        }

        foreach (var node in closedNodesBuffer)
        {
            Debug.Log($"Close : {node.name}");
            CloseNode(node);
        }

        openNodesBuffer.Clear();
        closedNodesBuffer.Clear();
    }

    private void ExpandNode(NodeObject node)
    {
        if (IsCloseNode(node))
        {
            return;
        }

        for (int i = 0; i < Direction.Length; i++)
        {
            EJointType currentDirJoint = node.GetJoint(i);
            if (currentDirJoint != EJointType.None)
            {
                Vector2Int neighborPosition = node.Position + Direction[i];
                if (entireNodes.ContainsKey(neighborPosition))
                {
                    var neighbor = entireNodes[neighborPosition];
                    if (IsOpenNode(neighbor) || IsCloseNode(neighbor))
                    {
                        continue;
                    }
                    else
                    {
                        openNodesBuffer.Add(neighbor);
                    }
                }
            }
        }

        closedNodesBuffer.Add(node);
    }

    private bool IsOpenNode(NodeObject node)
    {
        return openNodes.Contains(node);
    }

    private bool IsCloseNode(NodeObject node)
    {
        return closedNodes.Contains(node);
    }

#if UNITY_EDITOR
    public void Save(string path, MapModel mapData)
    {
        var nodes = GetComponentsInChildren<NodeObject>();
        mapData.Nodes = new NodeModel[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            mapData.Nodes[i].Convert(nodes[i]);
        }

        var save = JObject.FromObject(mapData).ToString(Formatting.Indented);
        File.WriteAllText(path, save);
    }
#endif
}