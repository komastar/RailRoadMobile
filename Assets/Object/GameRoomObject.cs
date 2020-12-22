using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Object
{
    public class GameRoomObject : MonoBehaviour
    {
        private GameManager gameManager;
        private NetworkManager netManager;

        private Coroutine updateGameRoomCoroutine;

        public Button confirmButton;
        public Button cancelButton;
        public Text gameCodeText;
        public Text readyInfoText;

        public Action onDisable;

        private void Awake()
        {
            gameManager = GameManager.Get();
            netManager = NetworkManager.Get();
            confirmButton.onClick.AddListener(OnClickConfirmButton);
            cancelButton.onClick.AddListener(OnClickCancelButton);
        }

        private void Start()
        {
            if (true == gameManager.IsSoloPlay())
            {
                Close();
            }
            else
            {
                updateGameRoomCoroutine = StartCoroutine(UpdateGameRoomCoroutine());
            }
        }

        private void OnDisable()
        {
            StopUpdateGameRoom();
            onDisable?.Invoke();
        }

        private void StopUpdateGameRoom()
        {
            if (!ReferenceEquals(null, updateGameRoomCoroutine))
            {
                StopCoroutine(updateGameRoomCoroutine);
            }
        }

        private void OnDestroy()
        {
            StopUpdateGameRoom();
        }

        private IEnumerator UpdateGameRoomCoroutine()
        {
            string gameCode = gameManager.GameRoom.GameCode;
            while (true)
            {
                netManager.GetRequest(UrlTable.GetFindGameUrl(gameCode)
                    , (response) =>
                    {
                        var game = GameRoomModel.Parse(response);
                        SetGameRoom(game);
                    });
                yield return new WaitForSecondsRealtime(.5f);
            }
        }

        public void SetGameRoom(GameRoomModel game)
        {
            if (null != game)
            {
                gameCodeText.text = game.GameCode;
                SetReadyInfo(game);
            }
        }

        public void SetReadyInfo(GameRoomModel game)
        {
            readyInfoText.text = $"{game.UserCount} / {game.MaxUserCount}";
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnClickConfirmButton()
        {
            string gameCode = gameManager.GameRoom.GameCode;
            netManager.GetRequest(UrlTable.GetStartGameUrl(gameCode)
                , (response) =>
                {
                    var gameRoom = GameRoomModel.Parse(response);
                    if (true == gameRoom.IsOpen)
                    {
                        Log.Info("NOT READY");
                    }
                    else
                    {
                        Close();
                    }
                });
        }

        private void OnClickCancelButton()
        {
            onDisable = null;
            var gameRoom = gameManager.GameRoom;
            netManager.GetRequest(UrlTable.GetExitGameUrl(gameRoom.GameCode, gameRoom.UserId)
                , (response) =>
                {
                    SceneManager.LoadScene("LobbyScene");
                });
        }
    }
}
