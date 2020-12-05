using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    private void Awake()
    {
        Manager.NetworkManager.Get();
        Manager.GameManager.Get();
    }

    public void TouchToStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
