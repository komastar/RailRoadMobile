using UnityEngine;

[SelectionBase]
public class NodeObject : MonoBehaviour
{
    public int routeId;
    public int direction = 0;
    public NodeObject[] neighbors;
    public Vector2Int Position;
    public SpriteRenderer routeRenderer;

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

    public void SetupNode(int id, Sprite sprite)
    {
        name = sprite == null ? "EmptyNode" : sprite.name;
        routeId = id;
        routeRenderer.sprite = sprite;
    }

    public void ResetNode()
    {
        name = "EmptyNode";
        routeId = 0;
        direction = 0;
        routeRenderer.sprite = null;
        UpdateRotation();
    }
}