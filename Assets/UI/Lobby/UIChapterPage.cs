using System.Collections;
using UnityEngine;

namespace Assets.UI.Lobby
{
    public class UIChapterPage : MonoBehaviour
    {
        public string Name;
        public UIStageButton stageButtonPrefab;

        public void SetName(string name)
        {
            this.name = name;
            Name = Manager.DataManager.Get().Localize("Chapter", name);
        }
    }
}