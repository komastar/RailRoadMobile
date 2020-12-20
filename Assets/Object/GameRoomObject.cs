using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Object
{
    public class GameRoomObject : MonoBehaviour
    {
        private GameManager gameManager;

        public Button confirmButton;
        public Button cancelButton;
        public Text gameCodeText;
        public Text readyInfoText;

        public Action onDisable;

        private Coroutine updateGameRoomCoroutine;

        private void Awake()
        {
            gameManager = GameManager.Get();
            confirmButton.onClick.AddListener(OnClickConfirmButton);
            cancelButton.onClick.AddListener(OnClickCancelButton);
        }

        private void Start()
        {
            var game = gameManager.GameRoom;
            if (GameCode.SoloPlay == game.GameCode)
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
            while (true)
            {
                var game = NetworkManager.FindGame(gameManager.GameRoom.GameCode);
                SetGameRoom(game);

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
            var gameRoom = NetworkManager.StartGame(gameManager.GameRoom.GameCode);
            if (true == gameRoom.IsOpen)
            {
                Log.Info("NOT READY");
            }
            else
            {
                Close();
            }
        }

        private void OnClickCancelButton()
        {
            onDisable = null;
            NetworkManager.ExitGame(gameManager.GameRoom.GameCode, gameManager.GameRoom.UserId);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
