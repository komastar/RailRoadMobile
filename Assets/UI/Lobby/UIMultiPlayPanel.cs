using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Assets.Foundation.UI.Common;
using Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UIMultiPlayPanel : UIPanel
    {
        private GameManager gameManager;
        private NetworkManager netManager;

        public InputField inputGameCode;
        public Button joinGameButton;
        public Button createGameButton;
        public Slider maxUserCountSlider;
        public Text maxUserCountText;
        public Text gameCodeText;

        public override void Setup()
        {
            gameManager = GameManager.Get();
            netManager = NetworkManager.Get();

            joinGameButton.onClick.AddListener(OnClickJoinGameButton);
            createGameButton.onClick.AddListener(OnClickCreateGameButton);
            maxUserCountSlider.onValueChanged.AddListener(OnMaxUserCountChange);
        }

        public void OnClickJoinGameButton()
        {
            string gameCode = inputGameCode.text;
            Log.Info(gameCode);
            netManager.GetRequest(UrlTable.GetJoinGameUrl(gameCode)
                , (response) =>
                {
                    var game = GameRoomModel.Parse(response);
                    if (null != game)
                    {
                        gameManager.GameRoom = game;
                        gameManager.GameUserId = game.UserId;
                        joinGameButton.GetComponentInChildren<Text>().color = Color.green;

                        StartCoroutine(StartGame());
                    }
                    else
                    {
                        joinGameButton.GetComponentInChildren<Text>().color = Color.red;
                    }
                });
        }

        public void OnClickCreateGameButton()
        {
            int maxUserCount = int.Parse(maxUserCountText.text);
            netManager.GetRequest(UrlTable.GetCreateGameUrl(maxUserCount),
                (response) =>
                {
                    var game = GameRoomModel.Parse(response);
                    if (null != game)
                    {
                        gameManager.GameRoom = game;
                        gameManager.GameUserId = game.OwnerUserId;
                        gameCodeText.text = game.GameCode;
                        StartCoroutine(StartGame());
                    }
                });
        }

        public void OnMaxUserCountChange(float value)
        {
            maxUserCountText.text = $"{value:0}";
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            SceneManager.LoadScene("GameScene");
        }

        public override void Close()
        {
            base.Close();
        }
    }
}