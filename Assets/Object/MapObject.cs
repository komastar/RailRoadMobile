using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public NodeObject nodePrefab;

    public void MakeMap(MapModel mapData)
    {
        Vector2Int mapSize = mapData.MapSize.ToVector2Int();
        float nodeSize = mapData.NodeSize;
        float offsetX = (nodeSize * 0.5f) * (mapData.MapSize.x - 1);
        float offsetY = (nodeSize * 0.5f) * (mapData.MapSize.y - 1);

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                var newNode = Instantiate(nodePrefab, transform);
                newNode.name = "EmptyNode";
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
        Vector2Int mapSize = mapData.MapSize.ToVector2Int();
        float nodeSize = mapData.NodeSize;
        float offsetX = (nodeSize * 0.5f) * (mapData.MapSize.x - 1);
        float offsetY = (nodeSize * 0.5f) * (mapData.MapSize.y - 1);

        var nodes = mapData.Nodes;
        for (int i = 0; i < nodes.Length; i++)
        {
            var node = nodes[i];
            var newNode = Instantiate(nodePrefab, transform);
            if (node.Id != 0)
            {
                newNode.SetupNode(node.Id, spriteData[routeData[node.Id].Name]);
            }
            else
            {
                newNode.SetupNode(node.Id, null);
            }
            newNode.transform.localPosition = new Vector3(node.Position.x - offsetX, node.Position.y - offsetY, 0f);
            newNode.transform.localScale = Vector3.one * nodeSize;
            newNode.Position = node.Position.ToVector2Int();
            newNode.Rotate((int)node.Direction);
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