
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour, IPointerClickHandler
{
    public Text scoreText;

    public Action onClose;

    public int Score;

    public void Init()
    {
        onClose = null;
        Close();
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        Score = score;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Close();
        onClose?.Invoke();
    }
}