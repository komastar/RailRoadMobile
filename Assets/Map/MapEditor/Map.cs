using Assets.Constant;
using Assets.Object;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Map.MapEditor
{
    public class Map : MonoBehaviour
    {
        public NodeObject nodePrefab;
        public MapModel mapData;
        public Vector2Int mapSize;
        public float nodeSize;
        public string mapName;

        public void Clear()
        {
            var children = GetComponentsInChildren<NodeObject>();
            for (int i = 0; i < children.Length; i++)
            {
                DestroyImmediate(children[i].gameObject);
            }
        }

        public void Save()
        {
            string saveName = mapData.Name;
            if (string.IsNullOrEmpty(saveName))
            {
                saveName = mapName;
                mapData.NodeSize = 1f;
                mapData.MapSize = new GridInt(mapSize.x, mapSize.y);
            }

            var nodes = GetComponentsInChildren<NodeObject>();
            mapData.Nodes = new NodeModel[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                mapData.Nodes[i].Convert(nodes[i]);
            }

            var save = JObject.FromObject(mapData).ToString(Formatting.Indented);
            string path = $"{Application.dataPath}/Data/.Map";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = $"{path}/{saveName}.json";

            File.WriteAllText(path, save);
        }

        public void Generate()
        {
            mapSize.x = mapData.MapSize.X;
            mapSize.y = mapData.MapSize.Y;
            mapName = mapData.Name;
            nodeSize = mapData.NodeSize;

            float offsetX = (nodeSize * 0.5f) * (mapData.MapSize.X - 1);
            float offsetY = (nodeSize * 0.5f) * (mapData.MapSize.Y - 1);
            for (int y = 0; y < mapData.MapSize.Y; y++)
            {
                for (int x = 0; x < mapData.MapSize.X; x++)
                {
                    var newNode = Instantiate(nodePrefab, transform);
                    newNode.name = "EmptyNode";
                    newNode.transform.localPosition = new Vector3(x * nodeSize - offsetX, y * nodeSize - offsetY, 0);
                    newNode.Position = new GridInt(x, y);
                }
            }

            Camera.main.orthographicSize = Math.Max(mapSize.x * nodeSize, mapSize.y * nodeSize);
        }

        public void Reset()
        {
            var selected = GetAllSelected();
            if (selected.Count > 0)
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].ResetNode();
                }
            }
        }

        public void SetRoute(int id)
        {
            var selected = GetAllSelected();
            if (selected.Count > 0)
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].SetupNode(id);
                }
            }
        }

        public void Rotate()
        {
            var selected = GetAllSelected();
            if (selected.Count > 0)
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].Rotate();
                }
            }
        }

        private List<NodeObject> GetAllSelected()
        {
            var objs = Selection.objects;
            List<NodeObject> nodes = new List<NodeObject>();
            for (int i = 0; i < objs.Length; i++)
            {
                nodes.Add((objs[i] as GameObject).GetComponent<NodeObject>());
            }

            return nodes;
        }
    }
}