
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour, IPointerClickHandler
{
    public Text pvpResultText;
    public Text totalScoreText;
    public Text networkScoreText;
    public Text railScoreText;
    public Text roadScoreText;
    public Text penaltyScoreText;

    public Action onClose;

    public ScoreViewModel scoreViewModel;

    public void Init()
    {
        onClose = null;
        Close();
    }

    public void SetPvpResult(string result)
    {
        pvpResultText.gameObject.SetActive(!string.IsNullOrEmpty(result));
        pvpResultText.text = result;
    }

    public void SetScore(ScoreViewModel score)
    {
        scoreViewModel = score;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        totalScoreText.text = scoreViewModel.TotalScore.ToString();
        networkScoreText.text = scoreViewModel.NetworkScore.ToString();
        railScoreText.text = scoreViewModel.RailScore.ToString();
        roadScoreText.text = scoreViewModel.RoadScore.ToString();
        string minus = scoreViewModel.PenaltyScore > 0 ? "-" : "";
        penaltyScoreText.text = $"{minus}{scoreViewModel.PenaltyScore + scoreViewModel.ConstructFailScore}";
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

    public void AddContructFailCount(int contructFailCount)
    {
        scoreViewModel.ConstructFailScore += contructFailCount;
    }
}