using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Text versionText;

    private void Awake()
    {
        Manager.NetworkManager.Get();
        Manager.GameManager.Get();
        versionText.text = $"{Application.version}";
    }

    public void TouchToStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
