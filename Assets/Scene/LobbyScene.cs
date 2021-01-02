using Assets.Foundation.UI.PopUp;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    public UIPopUpPanel popUpPanel;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            await ExitGame();
        }
    }

    private async Task ExitGame()
    {
        var confirm = (UIConfirmPopUp)popUpPanel.Open("Confirm");
        confirm.titleText.text = "게임을 종료하시겠습니까?";
        confirm.PositiveText.text = "예";
        confirm.NegativeText.text = "아니오";
        var result = await confirm.GetResult();
        if (true == result)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#elif UNITY_ANDROID
            Application.Quit();
#endif
        }
        else
        {
            popUpPanel.TurnOff();
        }
    }
}
