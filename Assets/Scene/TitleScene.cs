using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    public Text versionText;
    public Button startButton;

    private void Awake()
    {
        NetworkManager.Get();
        GameManager.Get().onAfterAuth +=
            (bool result) =>
            {
                startButton.interactable = result;
            };
        versionText.text = $"{Application.version}";
    }

    private void Start()
    {
#if UNITY_EDITOR
        startButton.interactable = true;
#endif
    }

    public void TouchToStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
