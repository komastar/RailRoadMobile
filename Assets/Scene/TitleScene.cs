using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    private void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }

    void Start()
    {
        Social.localUser.Authenticate((bool suc) =>
        {
            if (suc)
            {
                Log.Info($"Auth Suc");
                var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                GameManager.Get().SetAuthCode(authCode);
            }
            else
            {
                Log.Info($"Auth Fail");
            }
        });
    }

    public void TouchToStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
