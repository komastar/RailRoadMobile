using Manager;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Text versionText;
    public Button startButton;

    private PlayGamesPlatform gpgs;

    private void Awake()
    {
        GameManager.Get();
        NetworkManager.Get();

#if UNITY_ANDROID
        Log.Info("GPGS config");
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .EnableSavedGames()
            .Build();

        Log.Info("GPGS Init");
        PlayGamesPlatform.InitializeInstance(config);

        Log.Info("GPGS Activate");
        gpgs = PlayGamesPlatform.Activate();

        Log.Info("Auth Start");
        gpgs.Authenticate(suc => OnAuthenticate(suc));
#endif
        versionText.text = $"{Application.version}";
    }

    private void Start()
    {
        startButton.interactable = true;
    }

#if UNITY_ANDROID
    private void OnAuthenticate(bool suc)
    {
        if (suc)
        {
            Log.Info($"Auth Suc");
        }
        else
        {
            Log.Info($"Auth Fail");
        }
    }
#endif

    public void TouchToStart()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
