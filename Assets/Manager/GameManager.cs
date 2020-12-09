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

        public void ReportScore(int score)
        {
            gpgs.ReportScore(score, StringTable.HighestScoreLB, OnReportScore);
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
    }
}