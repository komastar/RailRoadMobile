using UnityEngine;

public class UIScreenMaskObject : MonoBehaviour
{
    public Material material;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetRect(Input.mousePosition, new Vector2(100, 100));
        }
    }

    public void Init()
    {
        SetRect(Vector2.zero, Vector2.zero);
        SetColor(Color.black);
        SetAlpha(.5f);
        TurnOff();
    }

    public void SetRect(Vector2 position, Vector2 size)
    {
        Vector4 rectVector = new Vector4(
            position.x - size.x * .5f
            , position.y + size.y * .5f
            , position.x + size.x * .5f
            , position.y - size.y * .5f);
        material.SetVector("_Rect", rectVector);
    }

    public void SetColor(Color color)
    {
        material.SetColor("_Color", color);
    }

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}