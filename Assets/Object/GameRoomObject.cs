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

        private void OnDisable()
        {
            if (!ReferenceEquals(null, updateGameRoomCoroutine))
            {
                StopCoroutine(updateGameRoomCoroutine);
            }
            onDisable?.Invoke();
        }

        private void OnDestroy()
        {
            StopCoroutine(updateGameRoomCoroutine);
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

        public void SetGameRoom(GameRoomModel game)
        {
            gameCodeText.text = game.GameCode;
            SetReadyInfo(game);
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
            Close();
        }

        private void OnClickCancelButton()
        {
            onDisable = null;
            NetworkManager.ExitGame(GameManager.Get().GameRoom.GameCode, GameManager.Get().GameRoom.UserId);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
