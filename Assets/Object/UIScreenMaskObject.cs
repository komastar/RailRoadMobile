using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenMaskObject : MonoBehaviour
{
    public Material material;
    public Image image;
    public RectTransform clickArea;
    public GameObject topDescPanel;
    public GameObject botDescPanel;
    public Text topDescText;
    public Text botDescText;

    public Action onClickActiveArea;
    public Action onDisable;

    private bool isBottom;

    private void Awake()
    {
        TurnOff();
        Init();
        botDescPanel.SetActive(false);
        topDescPanel.SetActive(false);
    }

    private void OnDisable()
    {
        onDisable?.Invoke();
    }

    public void Init()
    {
        image.material = Instantiate(material);
        SetRect(Vector2.zero, Vector2.zero);
        SetColor(Color.black);
        SetAlpha(.5f);
    }

    public void SetRect(Vector2 position, Vector2 size, int index = 0)
    {
        Vector4 rectVector = new Vector4(
            position.x - size.x * .5f
            , position.y + size.y * .5f
            , position.x + size.x * .5f
            , position.y - size.y * .5f);
        SetRect(index, rectVector);
        clickArea.position = position;
        clickArea.sizeDelta = size;
        isBottom = Screen.height * 0.5f < position.y;
        SetText(null);
    }

    public void ResetRect()
    {
        for (int i = 0; i < 4; i++)
        {
            ResetRect(i);
        }
    }

    public void ResetRect(int index)
    {
        SetRect(index, Vector4.zero);
    }

    public void SetRect(int index, Vector4 rect)
    {
        image.material.SetVector($"_Rect{index}", rect);
    }

    public void SetText(string text)
    {
        botDescPanel.SetActive(isBottom);
        topDescPanel.SetActive(!isBottom);
        if (false == string.IsNullOrEmpty(text))
        {
            botDescText.text = text;
            topDescText.text = text;
        }
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

    public void OnClickActiveArea()
    {
        onClickActiveArea?.Invoke();
    }
}