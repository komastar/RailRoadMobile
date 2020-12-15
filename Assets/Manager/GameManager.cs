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
        private PlayGamesPlatform gpgs;
        private string authCode;

        public Action<bool> onAfterAuth;

        public GameRoomModel GameRoom { get; set; }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

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
        }

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

        public void ReportScore(ScoreViewModel score)
        {
            gpgs.ReportScore(score.TotalScore, GPGSIds.leaderboard_highestscore, OnReportScore);
            gpgs.ReportProgress(GPGSIds.achievement_stageclear, 100.0f, OnReportProgress);
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
    }
}