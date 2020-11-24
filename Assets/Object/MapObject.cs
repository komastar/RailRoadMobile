using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapObject : MonoBehaviour, IGameActor
{
    public static readonly Vector2Int[] Direction = new Vector2Int[4]
    {
        Vector2Int.up
        , Vector2Int.left
        , Vector2Int.down
        , Vector2Int.right
    };

    public NodeObject nodePrefab;
    public HandObject hand;

    private DataManager dataManager;
    private SpriteManager spriteManager;

    private Dictionary<Vector2Int, NodeObject> entireNodes;
    private HashSet<NodeObject> openNodes;
    private HashSet<NodeObject> openNodesBuffer;
    private HashSet<NodeObject> closedNodes;
    private HashSet<NodeObject> closedNodesBuffer;

    private Dictionary<NodeObject, HashSet<Vector2Int>> connExits = new Dictionary<NodeObject, HashSet<Vector2Int>>();

    public int candidateId;
    public NodeObject candidateFix;
    public Action onFixPhaseExit;

    public int Id { get; set; }

    public void Init()
    {
        candidateId = 0;
        onFixPhaseExit = null;

        entireNodes = new Dictionary<Vector2Int, NodeObject>();
        openNodes = new HashSet<NodeObject>();
        closedNodes = new HashSet<NodeObject>();
        openNodesBuffer = new HashSet<NodeObject>();
        closedNodesBuffer = new HashSet<NodeObject>();

        dataManager = DataManager.Get();
        spriteManager = SpriteManager.Get();

        hand.onChangeHand = null;
        hand.onChangeHand += OnChangeHand;
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
            routeData = dataManager.RouteData;
        }

        if (spriteData == null)
        {
            spriteData = spriteManager.RouteSprites;
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
            newNode.onClick += OnClickNode;
            NewNode(newNode);
        }

        var orthoSize = Math.Max(mapSize.x * nodeSize, mapSize.y * nodeSize);
        Camera.main.orthographicSize = orthoSize;
    }

    public ScoreViewModel GetScore()
    {
        ScoreViewModel scoreViewModel = new ScoreViewModel();
        foreach (var node in entireNodes)
        {
            if (node.Value.NodeType == ENodeType.Entrance)
            {
                connExits.Add(node.Value, new HashSet<Vector2Int>() { node.Value.Position });
            }
        }

        foreach (var node in connExits)
        {
            for (int i = 0; i < 4; i++)
            {
                CollectNeighborRecursive(node.Key, node.Key.Neighbors[i], i);
            }
        }

        Dictionary<SortedSet<NodeObject>, int> exitScore = new Dictionary<SortedSet<NodeObject>, int>();
        HashSet<Vector2Int> passNode = new HashSet<Vector2Int>();
        foreach (var node in connExits)
        {
            if (passNode.Contains(node.Key.Position))
            {
                continue;
            }

            SortedSet<NodeObject> exitGroup = new SortedSet<NodeObject>();
            foreach (var comp in connExits)
            {
                if (node.Key.Position != comp.Key.Position)
                {
                    if (node.Value.Contains(comp.Key.Position) && comp.Value.Contains(node.Key.Position))
                    {
                        exitGroup.Add(node.Key);
                        exitGroup.Add(comp.Key);
                        passNode.Add(node.Key.Position);
                        passNode.Add(comp.Key.Position);
                    }
                }
            }
            if (exitGroup.Count > 1)
            {
                exitScore.Add(exitGroup, node.Value.Count);
            }
        }

        int networkScore = 0;
        foreach (var score in exitScore)
        {
            int scoreCalc = (score.Key.Count - 1) * 4;
            networkScore += scoreCalc;
            Debug.Log($"Score : {scoreCalc}");
        }

        Debug.Log($"TotalNetworkScore : {networkScore}");
        scoreViewModel.NetworkScore = networkScore;

        List<NodeObject> rails = new List<NodeObject>();
        List<NodeObject> roads = new List<NodeObject>();
        foreach (var node in entireNodes)
        {
            if (node.Value.NodeType == ENodeType.Entrance)
            {
                continue;
            }

            if (node.Value.IsRailRoute)
            {
                rails.Add(node.Value);
            }

            if (node.Value.IsRoadRoute)
            {
                roads.Add(node.Value);
            }
        }

        HashSet<NodeObject> visited = new HashSet<NodeObject>();
        int railScore = 0;
        foreach (var rail in rails)
        {
            visited.Clear();
            int currentRailScore = CalcLongestWayScoreRecursive(rail, EJointType.Rail, ref visited);
            railScore = railScore < currentRailScore ? currentRailScore : railScore;
        }

        int roadScore = 0;
        foreach (var road in roads)
        {
            visited.Clear();
            int currentRoadScore = CalcLongestWayScoreRecursive(road, EJointType.Road, ref visited);
            roadScore = roadScore < currentRoadScore ? currentRoadScore : roadScore;
        }

        Debug.Log($"TotalRailScore : {railScore}");
        Debug.Log($"TotalRoadScore : {roadScore}");
        scoreViewModel.RailScore = railScore;
        scoreViewModel.RoadScore = roadScore;

        int penaltyScore = 0;
        foreach (var node in closedNodes)
        {
            if (!IsEdgeNode(node))
            {
                for (int i = 0; i < 4; i++)
                {
                    EJointType joint = node.GetJoint(i);
                    if (joint != EJointType.None)
                    {
                        Vector2Int neighborPos = node.Position + Direction[i];
                        if (entireNodes.ContainsKey(neighborPos))
                        {
                            NodeObject neighbor = entireNodes[neighborPos];
                            if (!IsEdgeNode(neighbor))
                            {
                                EJointType nJoint = neighbor.GetJoint2(i);
                                if (joint != nJoint)
                                {
                                    penaltyScore++;
                                }
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"TotalFaultScore : {penaltyScore}");
        scoreViewModel.PenaltyScore = penaltyScore;
        scoreViewModel.Calculate();

        return scoreViewModel;
    }

    private void CollectNeighborRecursive(NodeObject root, NodeObject neighbor, int direction)
    {
        if (!ReferenceEquals(null, neighbor))
        {
            if (!connExits[root].Contains(neighbor.Position))
            {
                connExits[root].Add(neighbor.Position);
                for (int i = 0; i < 4; i++)
                {
                    if (!neighbor.RouteData.IsTransfer
                        && neighbor.GetJoint2(direction) != neighbor.GetJoint(i))
                    {
                        continue;
                    }
                    CollectNeighborRecursive(root, neighbor.Neighbors[i], i);
                }
            }
        }
    }

    private int CalcLongestWayScoreRecursive(NodeObject node, EJointType joint, ref HashSet<NodeObject> visited)
    {
        int highestScore = 0;
        visited.Add(node);
        if (node.NodeType == ENodeType.Entrance)
        {
            return 0;
        }

        for (int i = 0; i < 4; i++)
        {
            var joint1 = node.GetJoint(i);
            if (joint == joint1)
            {
                var neighbor = node.Neighbors[i];
                if (!ReferenceEquals(null, neighbor))
                {
                    if (!visited.Contains(neighbor))
                    {
                        var joint2 = neighbor.GetJoint2(i);
                        if (joint1 == joint2)
                        {
                            int score = CalcLongestWayScoreRecursive(neighbor, joint, ref visited);
                            highestScore = score > highestScore ? score : highestScore;
                        }
                    }
                }
            }
        }

        return 1 + highestScore;
    }

    public void Close()
    {
        foreach (var node in entireNodes)
        {
            CloseNode(node.Value);
        }
    }

    public void Clear()
    {
        connExits?.Clear();
        entireNodes?.Clear();
        openNodes?.Clear();
        closedNodes?.Clear();
        openNodesBuffer?.Clear();
        closedNodesBuffer?.Clear();

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
                case ENodeType.Normal:
                    break;
                case ENodeType.Entrance:
                    OpenNode(node.Value);
                    break;
                case ENodeType.None:
                case ENodeType.Wall:
                    CloseNode(node.Value);
                    break;
            }
        }
    }

#if UNITY_EDITOR
    public void SetCandidate(int id)
    {
        candidateId = id;
        candidateFix = null;
    }
#endif

    private void NewNode(NodeObject node)
    {
        entireNodes?.Add(node.Position, node);
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
        closedNodes.Add(node);
    }

    private void ExpandNodes()
    {
        foreach (var node in openNodes)
        {
            Debug.Log($"Expand : {node.name}");
            ExpandNode(node);
        }

        CommitOpenNode();
        CommitCloseNode();
    }

    private void CommitOpenNode()
    {
        foreach (var node in openNodesBuffer)
        {
            Debug.Log($"Open : {node.name}");
            OpenNode(node);
        }
        openNodesBuffer.Clear();
    }

    private void CommitCloseNode()
    {
        foreach (var node in closedNodesBuffer)
        {
            Debug.Log($"Close : {node.name}");
            CloseNode(node);
        }
        closedNodesBuffer.Clear();
    }

    private void ExpandNode(NodeObject node)
    {
        if (IsCloseNode(node) || node.IsEmpty())
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

    public void Rotate()
    {
        if (!ReferenceEquals(null, candidateFix))
        {
            candidateFix.Rotate();
        }
    }

    public void Flip()
    {
        if (!ReferenceEquals(null, candidateFix))
        {
            candidateFix.Flip();
        }
    }

    public void FixNode()
    {
        if (!ReferenceEquals(null, candidateFix))
        {
            if (IsConstructable(candidateFix))
            {
                FixNode(candidateFix);
                ExpandNodes();
                Debug.Log($"Fix suc : {candidateFix.name}");
                CloseNode(candidateFix);
                hand.DisposeNode();
                candidateId = 0;
                candidateFix = null;
                onFixPhaseExit?.Invoke();
            }
            else
            {
                Debug.Log($"Fix fail : {candidateFix.name}");
            }
        }
    }

    private void FixNode(NodeObject node)
    {
        Vector2Int pos = node.Position;
        for (int i = 0; i < Direction.Length; i++)
        {
            EJointType joint = node.GetJoint(i);
            if (joint != EJointType.None)
            {
                if (node.Neighbors[i] == null)
                {
                    Vector2Int neighborPos = pos + Direction[i];
                    if (entireNodes.ContainsKey(neighborPos))
                    {
                        NodeObject neighbor = entireNodes[neighborPos];
                        EJointType nJoint = neighbor.GetJoint2(i);
                        if (joint == nJoint)
                        {
                            node.Neighbors[i] = neighbor;
                            neighbor.Neighbors[(i + 2) % 4] = node;
                        }
                    }
                }
            }
        }
    }

    private bool IsOpenNode(NodeObject node)
    {
        return openNodes.Contains(node);
    }

    private bool IsCloseNode(NodeObject node)
    {
        return closedNodes.Contains(node);
    }

    public void OnClickNode(NodeObject node)
    {
        if (node.NodeState != ENodeState.Open)
        {
            return;
        }

        if (!ReferenceEquals(null, candidateFix))
        {
            candidateFix.ResetNode();
        }

        candidateId = hand.GetDice();
        if (candidateId < 0)
        {
            Debug.Log($"No Candidate Node");

            return;
        }

        var route = dataManager.RouteData[candidateId];
        var sprite = spriteManager.RouteSprites[route.Name];
        node.SetupNode(route, sprite);
        Debug.Log($"Build : {route.Name}");
        candidateFix = node;
    }

    public bool IsConstructable(NodeObject node)
    {
        int closeConnectCount = 0;
        int connectCount = 0;
        for (int i = 0; i < Direction.Length; i++)
        {
            Vector2Int neighborPos = node.Position + Direction[i];
            var neighbor = entireNodes[neighborPos];
            if (neighbor.NodeType == ENodeType.Wall)
            {
                connectCount++;
                continue;
            }

            EJointType joint = node.GetJoint(i);
            EJointType neighborJoint = neighbor.GetJoint((i + 2) % 4);
            if (IsCloseNode(neighbor))
            {
                if (joint == EJointType.None)
                {
                    connectCount++;
                }
                else
                {
                    if (neighborJoint == EJointType.None)
                    {
                        connectCount++;
                    }

                    if (joint == neighborJoint)
                    {
                        connectCount++;
                        closeConnectCount++;
                    }
                }
            }
            else
            {
                connectCount++;
            }
        }

        Debug.Log($"Connect : {connectCount} / {closeConnectCount}");

        return (connectCount == 4) && (closeConnectCount > 0);
    }

    private bool IsEdgeNode(NodeObject node)
    {
        return (node.NodeType == ENodeType.Wall || node.NodeType == ENodeType.Entrance);
    }

    public void Init(int id)
    {
        throw new NotImplementedException();
    }

    private void OnChangeHand()
    {
        if (!ReferenceEquals(null, candidateFix))
        {
            candidateFix.ResetNode();
            candidateFix = null;
        }
    }

#if UNITY_EDITOR
    public void Save(string path, MapModel mapData)
    {
        var nodes = GetComponentsInChildren<NodeObject>();
        mapData.Nodes = new NodeModel[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            mapData.Nodes[i] = new NodeModel();
            mapData.Nodes[i].Convert(nodes[i]);
        }

        var save = JObject.FromObject(mapData).ToString(Formatting.Indented);
        File.WriteAllText(path, save);
    }

    public NodeObject GetNode(Vector2Int pos)
    {
        return entireNodes[pos];
    }
#endif
}