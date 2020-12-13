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
        public Button confirmButton;
        public Button cancelButton;
        public Text gameCodeText;
        public Text readyInfoText;

        public Action onDisable;

        private Coroutine updateGameRoomCoroutine;

        private void Awake()
        {
            confirmButton.onClick.AddListener(OnClickConfirmButton);
            cancelButton.onClick.AddListener(OnClickCancelButton);
        }

        private void Start()
        {
            var game = GameManager.Get().GameRoom;
            if (GameCode.SoloPlay == game.GameCode)
            {
                Close();
            }
            else
            {
                updateGameRoomCoroutine = StartCoroutine(UpdateGameRoomCoroutine());
            }
        }

        private IEnumerator UpdateGameRoomCoroutine()
        {
            while (true)
            {
                var game = NetworkManager.GetGame(GameManager.Get().GameRoom.GameCode);
                SetGameRoom(game);

                yield return new WaitForSecondsRealtime(.5f);
            }
        }

        private void OnDisable()
        {
            StopCoroutine(updateGameRoomCoroutine);
            onDisable?.Invoke();
        }

        public void SetGameRoom(GameModel game)
        {
            gameCodeText.text = game.GameCode;
            SetReadyInfo(game);
        }

        public void SetReadyInfo(GameModel game)
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
            Close();
        }

        private void OnClickCancelButton()
        {
            NetworkManager.ExitGame(GameManager.Get().GameRoom.GameCode);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
