using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public InputField inputGameCode;
    public Button joinGameButton;
    public Button createGameButton;
    public Slider maxUserCountSlider;
    public Text maxUserCountText;
    public Text gameCodeText;

    private void Awake()
    {
        joinGameButton.onClick.AddListener(OnClickJoinGameButton);
        createGameButton.onClick.AddListener(OnClickCreateGameButton);
        maxUserCountSlider.onValueChanged.AddListener(OnMaxUserCountChange);
    }

    public void OnClickJoinGameButton()
    {
        string gameCode = inputGameCode.text;
        Log.Info(gameCode);
        if (NetworkManager.JoinGame(gameCode))
        {
            joinGameButton.GetComponentInChildren<Text>().color = Color.green;
        }
        else
        {
            joinGameButton.GetComponentInChildren<Text>().color = Color.red;
        }
    }

    public void OnClickCreateGameButton()
    {
        gameCodeText.text = NetworkManager.CreateGame(int.Parse(maxUserCountText.text));
    }

    public void OnMaxUserCountChange(float value)
    {
        maxUserCountText.text = $"{value:0}";
    }
}
