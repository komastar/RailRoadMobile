using Assets.Foundation.Constant;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private PlayGamesPlatform gpgs;
        private string authCode;

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
        }

        public void SetAuthCode(string authCode)
        {
            this.authCode = authCode;
        }

        public void ReportScore(ScoreViewModel score)
        {
            gpgs.ReportScore(score.TotalScore, StringTable.HighestScoreLB, OnReportScore);
            gpgs.ReportScore(score.NetworkScore, StringTable.HighestScoreLBTest, (bool result) =>
            {
                gpgs.ShowLeaderboardUI(StringTable.HighestScoreLBTest, OnShowLeaderBoardUI);
            });
        }

        private void OnReportScore(bool value)
        {
            if (value)
            {
                Log.Info("Report SUC");
                //gpgs.ShowLeaderboardUI(StringTable.HighestScoreLB, LeaderboardTimeSpan.AllTime, OnShowLeaderBoardUI);
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