using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        private PlayGamesPlatform gpgs;
        private string authCode;

        public Action<bool> onAfterAuth;
#endif
        private PlayerSaveModel playerSaveData;

        private GameRoomModel gameRoom;
        public GameRoomModel GameRoom
        {
            get
            {
                if (null == gameRoom)
                {
                    gameRoom = GameRoomModel.GetSoloPlay();
                }

                return gameRoom;
            }
            set
            {
                gameRoom = value;
            }
        }
        public string GameUserId { get; set; }

        public ChapterModel CurrentChapter { get; set; }
        public StageModel CurrentStage { get; set; }

        private void Awake()
        {
            UrlTable.IsRemote = false;
            Init();
        }

        private void Init()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            LoadPlayerData();

#if UNITY_EDITOR
#elif UNITY_ANDROID
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
                .EnableSavedGames()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            Log.Info("Auth Start");
            gpgs = PlayGamesPlatform.Instance;
            gpgs.Authenticate(suc => OnAuthenticate(suc), false);
            gpgs.SetGravityForPopups(Gravity.CENTER_HORIZONTAL);
#endif
        }

        private void LoadPlayerData()
        {
            string savePath = $"{Application.persistentDataPath}/Player.json";
            if (File.Exists(savePath))
            {
                playerSaveData = JObject.Parse(File.ReadAllText(savePath)).ToObject<PlayerSaveModel>();
            }
            else
            {
                playerSaveData = PlayerSaveModel.MakeNewPlayerData();
                SavePlayerData();
            }
        }

        private void SavePlayerData()
        {
            string savePath = $"{Application.persistentDataPath}/Player.json";
            File.WriteAllText(savePath, JObject.FromObject(playerSaveData).ToString(Newtonsoft.Json.Formatting.Indented));
        }

#if UNITY_EDITOR
#elif UNITY_ANDROID
        private void OnAuthenticate(bool suc)
        {
            if (suc)
            {
                Log.Info($"Auth Suc");
                var authCode = gpgs.GetServerAuthCode();
                SetAuthCode(authCode);
                Log.Info($"Auth Code : {authCode}");
            }
            else
            {
                Log.Info($"Auth Fail");
            }
            onAfterAuth?.Invoke(suc);
        }

        public void SetAuthCode(string authCode)
        {
            this.authCode = authCode;
        }
#endif

        public void ReportScore(ScoreViewModel score)
        {
            if (IsSoloPlay()
                && 0 < score.TotalScore)
            {
                ClearStage(score.StageId);
            }
#if UNITY_EDITOR
#elif UNITY_ANDROID
            gpgs.ReportScore(Math.Abs(score.TotalScore), GPGSIds.leaderboard_highestscore, OnReportScore);
            gpgs.ReportProgress(GPGSIds.achievement_stageclear, 100.0f, OnReportProgress);
#endif
        }

#if UNITY_EDITOR
#elif UNITY_ANDROID
        private void OnReportProgress(bool isDone)
        {
            if (isDone)
            {
                Log.Info("Achievement Report SUC");
            }
            else
            {
                Log.Info("Achievement Report FAIL");
            }
        }

        private void OnReportScore(bool value)
        {
            if (value)
            {
                Log.Info("Report SUC");
            }
            else
            {
                Log.Info("Report FAIL");
            }
        }

        private void OnShowLeaderBoardUI(UIStatus obj)
        {
            Log.Info(obj);
        }
#endif

        public bool IsSoloPlay()
        {
            return (GameCode.SoloPlay == GameRoom.GameCode);
        }

        public void ClearStage(int stageId)
        {
            playerSaveData.ClearStage(stageId);
            SavePlayerData();
        }

        public bool IsClearStage(int stageId)
        {
            return playerSaveData.IsClearStage(stageId);
        }
    }
}