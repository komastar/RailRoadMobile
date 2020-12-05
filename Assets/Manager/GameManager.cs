using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
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
            PlayGamesPlatform.Instance.Authenticate(suc => OnAuthenticate(suc), false);
        }

        private void OnAuthenticate(bool suc)
        {
            if (suc)
            {
                Log.Info($"Auth Suc");
                var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
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
    }
}