using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
#if UNITY_ANDROID
        private PlayGamesPlatform gpgs;
        private string authCode;

        public Action<bool> onAfterAuth;
#endif

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
#if UNITY_ANDROID
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

#if UNITY_ANDROID
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
#if UNITY_ANDROID
            gpgs.ReportScore(Math.Abs(score.TotalScore), GPGSIds.leaderboard_highestscore, OnReportScore);
            gpgs.ReportProgress(GPGSIds.achievement_stageclear, 100.0f, OnReportProgress);
#endif
        }

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

        public bool IsSoloPlay()
        {
            return (GameCode.SoloPlay == GameRoom.GameCode);
        }
    }
}