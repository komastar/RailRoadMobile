using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    private GameManager gameManager;

    public Button soloPlayButton;
    public InputField inputGameCode;
    public Button joinGameButton;
    public Button createGameButton;
    public Slider maxUserCountSlider;
    public Text maxUserCountText;
    public Text gameCodeText;

    private void Awake()
    {
        gameManager = GameManager.Get();

        soloPlayButton.onClick.AddListener(OnClickSoloPlayGameButton);
        joinGameButton.onClick.AddListener(OnClickJoinGameButton);
        createGameButton.onClick.AddListener(OnClickCreateGameButton);
        maxUserCountSlider.onValueChanged.AddListener(OnMaxUserCountChange);
    }

    public void OnClickJoinGameButton()
    {
        string gameCode = inputGameCode.text;
        Log.Info(gameCode);
        var game = NetworkManager.JoinGame(gameCode);
        if (null != game)
        {
            gameManager.GameRoom = game;
            joinGameButton.GetComponentInChildren<Text>().color = Color.green;

            StartCoroutine(StartGame());
        }
        else
        {
            joinGameButton.GetComponentInChildren<Text>().color = Color.red;
        }
    }

    public void OnClickCreateGameButton()
    {
        var game = NetworkManager.CreateGame(int.Parse(maxUserCountText.text));
        gameManager.GameRoom = game;
        gameCodeText.text = game.GameCode;
        game = NetworkManager.JoinGame(game.GameCode);
        if (null != game)
        {
            StartCoroutine(StartGame());
        }
    }

    public void OnMaxUserCountChange(float value)
    {
        maxUserCountText.text = $"{value:0}";
    }

    public void OnClickSoloPlayGameButton()
    {
        gameManager.GameRoom = GameRoomModel.GetSoloPlay();
        SceneManager.LoadScene("GameScene");
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        SceneManager.LoadScene("GameScene");
    }
}
