﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class NodeObject : MonoBehaviour, IActor
{
    public int Id { get => routeId; set => routeId = value; }
    public int routeId;
    public int direction = 0;
    public bool isFlip = false;
    public ENodeType NodeType = ENodeType.Normal;
    public ENodeState NodeState = ENodeState.None;
    public EJointType[] Joints;
    public Vector2Int Position;
    public SpriteRenderer routeRenderer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + routeRenderer.transform.up * -.4f, transform.position + routeRenderer.transform.up * .4f);
        Gizmos.DrawLine(transform.position + routeRenderer.transform.right * .25f, transform.position + routeRenderer.transform.up * .4f);
        Gizmos.DrawLine(transform.position + routeRenderer.transform.right * -.25f, transform.position + routeRenderer.transform.up * .4f);

        switch (NodeState)
        {
            case ENodeState.Open:
                Gizmos.color = Color.green;
                break;
            case ENodeState.Close:
                Gizmos.color = Color.red;
                break;
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

    public void Flip()
    {
        isFlip = !isFlip;
    }

    public void Rotate(int dir = -1)
    {
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
        routeRenderer.transform.rotation = Quaternion.Euler(rotation);
    }

    public void SetupNode(RouteModel route, Sprite sprite)
    {
        routeId = route.Id;
        name = route.Name;
        routeRenderer.sprite = sprite;
    }

    public void SetupNode(NodeModel nodeData, RouteModel routeData, Sprite sprite)
    {
        SetupNode(routeData, sprite);
        NodeType = nodeData.NodeType;
        Position = nodeData.Position;
        Rotate((int)nodeData.Direction);
        Joints = routeData.Joints;
    }

    public void ResetNode()
    {
        name = "EmptyNode";
        routeId = 0;
        direction = 0;
        routeRenderer.sprite = null;
        UpdateRotation();
    }

    public EJointType GetJoint(int index)
    {
        int joint = 4 - direction + index;
        joint = isFlip ? joint + 2 : joint;
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
}