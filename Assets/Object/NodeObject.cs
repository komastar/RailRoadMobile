using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Manager;

[SelectionBase]
public class NodeObject : ObservablePointerClickTrigger, INode, IComparable<NodeObject>
{
    private readonly static Vector3 flipScale = new Vector3(-1f, 1f, 1f);

    private DataManager dataManager;
    private SpriteManager spriteManager;

    [SerializeField]
    private int id;
    [SerializeField]
    private int round;
    public int Round
    {
        get => round;
        set
        {
            round = value;
            if (0 == round)
            {
                RoundText.gameObject.SetActive(false);
            }
            else
            {
                RoundText.gameObject.SetActive(true);
            }
            RoundText.text = round.ToString();
        }
    }
    public int Id { get => id; set => id = value; }
    public int direction = 0;
    public bool isFlip = false;
    [SerializeField]
    private ENodeType nodeType;
    public ENodeType NodeType { get => nodeType; set => nodeType = value; }
    public ENodeState NodeState { get; set; }
    [SerializeField]
    private string floor;
    public string Floor { get => floor; set => floor = value; }
    public EJointType[] Joints;
    public Vector2Int Position { get; set; }
    public EDirection Direction { get => (EDirection)direction; set => direction = (int)value; }

    public TextMeshPro RoundText;
    public SpriteRenderer TileRenderer;
    public SpriteRenderer RouteRenderer;

    public Func<NodeObject, int> onClick;
    public Action<NodeObject> onResetBefore;
    public Action onResetAfter;

    public NodeObject[] Neighbors;
    public RouteModel RouteData;

    public bool IsRailRoute = false;
    public bool IsRoadRoute = false;

    private void Awake()
    {
        round = 0;
        var clickObserver = OnPointerClickAsObservable();
        clickObserver
            .Buffer(clickObserver.Throttle(TimeSpan.FromMilliseconds(250)))
            .Where(xs => xs.Count > 0)
            .Subscribe(
            xs =>
            {
                if (xs.Count == 2)
                {
                    ResetNode();
                }
            });
    }

    #region GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + RouteRenderer.transform.up * -.4f, transform.position + RouteRenderer.transform.up * .4f);
        Gizmos.DrawLine(transform.position + RouteRenderer.transform.right * .25f, transform.position + RouteRenderer.transform.up * .4f);
        Gizmos.DrawLine(transform.position + RouteRenderer.transform.right * -.25f, transform.position + RouteRenderer.transform.up * .4f);

        switch (NodeState)
        {
            case ENodeState.Open:
                Gizmos.color = Color.green;
                break;
            case ENodeState.Close:
                Gizmos.color = Color.red;
                break;
            case ENodeState.None:
            case ENodeState.Count:
            default:
                Gizmos.color = Color.black;
                break;
        }
        Gizmos.DrawCube(transform.position + (Vector3.left + Vector3.down) * .40f, Vector3.one * .2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * .25f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke(this);
        base.OnPointerClick(eventData);
    }
    #endregion

    public void Init(int id)
    {
        NodeState = ENodeState.None;
        Id = id;
        dataManager = DataManager.Get();
        spriteManager = SpriteManager.Get();
        RouteData = dataManager.RouteData[id];
        RouteRenderer.sprite = spriteManager.GetSprite(RouteData.Name);
        Neighbors = new NodeObject[4];
        IsRailRoute = false;
        IsRoadRoute = false;
    }

    public void Flip()
    {
        if (!RouteData.IsFlip)
        {
            return;
        }

        isFlip = !isFlip;
        if (isFlip)
        {
            RouteRenderer.transform.localScale = flipScale;
        }
        else
        {
            RouteRenderer.transform.localScale = Vector3.one;
        }

        var tempJoint = Joints[1];
        Joints[1] = Joints[3];
        Joints[3] = tempJoint;
    }

    public void Rotate(int dir = -1)
    {
        if (!RouteData.IsRotate)
        {
            return;
        }

        if (dir == -1)
        {
            direction++;
        }
        else
        {
            direction = dir;
        }
        direction %= 4;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3 rotation = new Vector3(0f, 0f, direction * 90f);
        RouteRenderer.transform.rotation = Quaternion.Euler(rotation);
    }

    public void SetupNode(RouteModel route, Sprite routeSprite, Sprite floorSprite = null)
    {
        Id = route.Id;
        RouteData = route;
        name = RouteData.Name;
        IsRailRoute = false;
        IsRoadRoute = false;
        NodeState = ENodeState.Open;
        Joints = new EJointType[4];
        onResetBefore = null;
        RouteData.Joints.CopyTo(Joints, 0);
        for (int i = 0; i < 4; i++)
        {
            if (Joints[i] == EJointType.Rail)
            {
                IsRailRoute = true;
            }

            if (Joints[i] == EJointType.Road)
            {
                IsRoadRoute = true;
            }
        }
        RouteRenderer.sprite = routeSprite;
        if (null != floorSprite)
        {
            TileRenderer.sprite = floorSprite;
        }

        if (Id == 0)
        {
            ResetNode();
        }
    }

    public void SetupFloor(Sprite sprite)
    {
        Floor = sprite.name;
        TileRenderer.sprite = sprite;
    }

    public void SetupNode(NodeModel nodeData, RouteModel routeData, Sprite routeSprite, Sprite floorSprite = null)
    {
        SetupNode(routeData, routeSprite, floorSprite);
        NodeType = nodeData.NodeType;
        Position = nodeData.Position;
        Rotate((int)nodeData.Direction);
        Neighbors = new NodeObject[4];
    }

    public void ResetNode()
    {
        onResetBefore?.Invoke(this);
        NodeState = ENodeState.None;
        name = "EmptyNode";
        Id = 0;
        Round = 0;
        direction = 0;
        RouteRenderer.sprite = null;
        Joints = new EJointType[4];
        UpdateRotation();
        RouteRenderer.transform.localScale = Vector3.one;
        IsRailRoute = false;
        IsRoadRoute = false;
        onResetAfter?.Invoke();

        onResetBefore = null;
        onResetAfter = null;
    }

    public void ReadyToTransfer()
    {
        onResetBefore = null;
        onResetAfter = null;
    }

    public EJointType GetJoint(int index)
    {
        int joint = 4 - direction + index;
        joint %= 4;

        return Joints[joint];
    }

    public EJointType GetJoint2(int index)
    {
        return GetJoint((index + 2) % 4);
    }

    public void Open()
    {
        NodeState = ENodeState.Open;
    }

    public void Close()
    {
        NodeState = ENodeState.Close;
    }

    public void Select()
    {
        TileRenderer.color = Color.green;
    }

    public void Deselect()
    {
        TileRenderer.color = Color.white;
    }

    public bool IsEmpty()
    {
        return Id == 0;
    }

    public int CompareTo(NodeObject other)
    {
        int pos = Position.x * 1000 + Position.y;
        int otherPos = other.Position.x * 1000 + other.Position.y;
        if (pos > otherPos)
        {
            return -1;
        }
        else if (pos == otherPos)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}