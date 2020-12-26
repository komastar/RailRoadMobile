using UnityEngine;
using UnityEngine.UI;

public class UIScreenMaskObject : MonoBehaviour
{
    public Material material;
    public Image image;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        image.material = Instantiate(material);
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
        image.material.SetVector("_Rect", rectVector);
    }

    public void SetColor(Color color)
    {
        image.material.SetColor("_Color", color);
    }

    public void SetAlpha(float alpha)
    {
        image.material.SetFloat("_Alpha", alpha);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}