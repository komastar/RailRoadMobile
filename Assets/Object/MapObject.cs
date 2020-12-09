using Manager;
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
    private Dictionary<NodeObject, HashSet<Vector2Int>> connExits;

    public int round;
    public DiceObject selectDice;
    public NodeObject selectedNode;
    public Action onFixPhaseExit;

    public int Id { get; set; }

    public void Init()
    {
        round = 0;
        selectDice = null;
        onFixPhaseExit = null;

        entireNodes = new Dictionary<Vector2Int, NodeObject>();
        connExits = new Dictionary<NodeObject, HashSet<Vector2Int>>();

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

        SetCamera(mapSize, nodeSize);
    }

    private static void SetCamera(Vector2Int mapSize, float nodeSize)
    {
        var orthoSize = Math.Max(mapSize.x * nodeSize, mapSize.y * nodeSize);
        float ratio = Screen.height / (float)Screen.width;
        orthoSize *= 1f + Mathf.Abs(ratio - 1.7f);
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
            spriteData = spriteManager.Sprites;
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
                if ("Wall" != routeData[node.Id].Name)
                {
                    sprite = spriteData[routeData[node.Id].Name];
                }
            }

            Sprite sprite2 = null;
            if (!string.IsNullOrEmpty(node.Floor)
                && spriteData.ContainsKey(node.Floor))
            {
                sprite2 = spriteData[node.Floor];
            }

            newNode.SetupNode(node, routeData[node.Id], sprite, sprite2);
            newNode.transform.localPosition = new Vector3(node.Position.x - offsetX, node.Position.y - offsetY, 0f);
            newNode.transform.localScale = Vector3.one * nodeSize;
            newNode.onClick += OnClickNode;
            NewNode(newNode);
        }

        SetCamera(mapSize, nodeSize);
    }

    public void NewRound(int roundCount)
    {
        round = roundCount;
    }

    public ScoreViewModel GetScore()
    {
        connExits.Clear();
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
            Log.Debug($"Score : {scoreCalc}");
        }

        Log.Debug($"TotalNetworkScore : {networkScore}");
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

        Log.Debug($"TotalRailScore : {railScore}");
        Log.Debug($"TotalRoadScore : {roadScore}");
        scoreViewModel.RailScore = railScore;
        scoreViewModel.RoadScore = roadScore;

        int penaltyScore = 0;
        foreach (var node in entireNodes.Values)
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

        Log.Debug($"TotalFaultScore : {penaltyScore}");
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

                    var newNeighbor = neighbor.Neighbors[i];
                    if (!ReferenceEquals(null, newNeighbor))
                    {
                        if (neighbor.GetJoint(i) == newNeighbor.GetJoint2(i))
                        {
                            CollectNeighborRecursive(root, newNeighbor, i);
                        }
                    }
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

    public void Clear()
    {
        selectedNode = null;
        connExits?.Clear();
        entireNodes?.Clear();

        var children = GetComponentsInChildren<NodeObject>();
        for (int i = 0; i < children.Length; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }
    }

    public void OpenMap()
    {
        NewRound(0);
        round = 1;
    }

#if UNITY_EDITOR
    public void SetCandidate(int id)
    {
        //selectDice = id;
        selectedNode = null;
    }
#endif

    private void NewNode(NodeObject node)
    {
        entireNodes?.Add(node.Position, node);
    }

    private void AddRoundNode(NodeObject node)
    {
        node.Round = round;
    }

    private void RemoveRoundNode(NodeObject node)
    {
        node.Round = 0;
    }

    public void Rotate()
    {
        if (!ReferenceEquals(null, selectedNode))
        {
            selectedNode.Rotate();
            FixNode();
        }
    }

    public void Flip()
    {
        if (!ReferenceEquals(null, selectedNode))
        {
            selectedNode.Flip();
            FixNode();
        }
    }

    public void FixNode()
    {
        if (!ReferenceEquals(null, selectedNode))
        {
            if (IsConstructable(selectedNode))
            {
                FixNode(selectedNode);
                Log.Debug($"Fix suc : {selectedNode.name}");
                //CloseNode(selectedNode);
            }
            else
            {
                Log.Debug($"Fix fail : {selectedNode.name}");
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

    public bool Fix()
    {
        if (0 < hand.GetDiceCount())
        {
            return false;
        }

        foreach (var node in entireNodes)
        {
            if (round == node.Value.Round)
            {
                if (!IsConstructable(node.Value))
                {
                    return false;
                }
            }
        }

        foreach (var node in entireNodes)
        {
            if (round == node.Value.Round)
            {
                node.Value.NodeState = ENodeState.Close;
            }
        }

        DeselectNode();

        return true;
    }

    public void CancelNode()
    {
        foreach (var node in entireNodes)
        {
            if (node.Value.Round == round)
            {
                node.Value.ResetNode();
            }
        }

        hand.Cancel();
        DeselectNode();
    }

    public int OnClickNode(NodeObject node)
    {
        if (selectedNode == node)
        {
            return 1;
        }

        if (node.NodeState == ENodeState.Close
            || node.NodeState == ENodeState.Open
            || node.NodeType != ENodeType.Normal)
        {
            return -1;
        }

        var diceFromHand = hand.GetDice();
        if (ReferenceEquals(null, diceFromHand))
        {
            return -1;
        }

        if (selectDice == diceFromHand)
        {
            if (!ReferenceEquals(null, selectedNode))
            {
                selectedNode.ReadyToTransfer();
                selectedNode.ResetNode();
            }
            var route = dataManager.RouteData[selectDice.DiceId];
            var sprite = spriteManager.Sprites[route.Name];
            node.SetupNode(route, sprite);
            AddRoundNode(node);
            LinkNodeAction(node);
            SelectNode(node);
            FixNode();

            return 0;
        }

        selectDice = diceFromHand;
        if (ReferenceEquals(null, selectDice))
        {
            if (node.Round == round)
            {
                SelectNode(node);
            }
        }
        else
        {
            if (node.Round == round)
            {
                hand.Return(node);
            }
            var route = dataManager.RouteData[selectDice.DiceId];
            var sprite = spriteManager.Sprites[route.Name];
            node.SetupNode(route, sprite);
            AddRoundNode(node);
            LinkNodeAction(node);
            Log.Debug($"Build : {route.Name} / {node.Position}");
            hand.DisposeNode();
            SelectNode(node);
        }

        FixNode();

        return 0;
    }

    private void LinkNodeAction(NodeObject node)
    {
        node.onResetBefore += hand.Return;
        node.onResetBefore += RemoveRoundNode;
        node.onResetAfter += DeselectNode;
        node.onResetAfter += hand.ResetHand;
        node.onResetAfter += ResetDice;
    }

    private void ResetDice()
    {
        selectDice = null;
    }

    private void SelectNode(NodeObject node)
    {
        DeselectNode();
        selectedNode = node;
        selectedNode.Select();
    }

    private void DeselectNode()
    {
        if (!ReferenceEquals(null, selectedNode))
        {
            selectedNode.Deselect();
        }
        selectedNode = null;
    }

    public bool IsConstructable(NodeObject node)
    {
        int matchCount = 0;
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
            EJointType neighborJoint = neighbor.GetJoint2(i);
            if (joint == EJointType.None
                || neighborJoint == EJointType.None)
            {
                connectCount++;
            }
            else if (joint == neighborJoint)
            {
                connectCount++;
                matchCount++;
            }
        }

        Log.Debug($"Connect : {connectCount} / {matchCount}");

        return (connectCount == 4) && (matchCount > 0);
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