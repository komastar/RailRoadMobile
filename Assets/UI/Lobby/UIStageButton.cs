using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Lobby
{
    public class UIStageButton : MonoBehaviour
    {
        public StageModel StageData;
        public Text StageNumberText;
        public Text StageStatusText;

        public void SetStageNumber(int value)
        {
            StageNumberText.text = $"{value}";
        }

        public void SetStageStatus(bool isClear)
        {
            StageStatusText.text = isClear ? "CLEAR" : "";
        }
    }
}