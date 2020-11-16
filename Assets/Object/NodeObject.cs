﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class NodeObject : MonoBehaviour, IGameActor, IPointerClickHandler
{
    private readonly static Vector3 flipScale = new Vector3(-1f, 1f, 1f);

    private DataManager dataManager;
    private SpriteManager spriteManager;

    public int Id { get => RouteData.Id; set => RouteData.Id = value; }
    public int direction = 0;
    public bool isFlip = false;
    public ENodeType NodeType = ENodeType.Normal;
    public ENodeState NodeState = ENodeState.None;
    public EJointType[] Joints;
    public Vector2Int Position;
    public SpriteRenderer RouteRenderer;
    public Action<NodeObject> onClick;

    public RouteModel RouteData;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }

    public void Init(int id)
    {
        dataManager = DataManager.Get();
        spriteManager = SpriteManager.Get();
        RouteData = dataManager.RouteData[id];
        RouteRenderer.sprite = spriteManager.GetSprite(RouteData.Name);
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
            transform.localScale = flipScale;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
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

    public void SetupNode(RouteModel route, Sprite sprite)
    {
        RouteData = route;
        name = RouteData.Name;
        Joints = RouteData.Joints;
        RouteRenderer.sprite = sprite;
        if (Id == 0)
        {
            ResetNode();
        }
    }

    public void SetupNode(NodeModel nodeData, RouteModel routeData, Sprite sprite)
    {
        SetupNode(routeData, sprite);
        NodeType = nodeData.NodeType;
        Position = nodeData.Position;
        Rotate((int)nodeData.Direction);
    }

    public void ResetNode()
    {
        name = "EmptyNode";
        Id = 0;
        direction = 0;
        RouteRenderer.sprite = null;
        UpdateRotation();
    }

    public EJointType GetJoint(int index)
    {
        if (isFlip)
        {
            if (index == 1 || index == 3)
            {
                index = (index + 2) % 4;
            }
        }

        int joint = 4 - direction + index;
        joint %= 4;

        return Joints[joint];
    }

    public void Open()
    {
        NodeState = ENodeState.Open;
    }

    public void Close()
    {
        NodeState = ENodeState.Close;
    }

    public bool IsEmpty()
    {
        return Id == 0;
    }
}