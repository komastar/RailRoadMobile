using Assets.Foundation.UI.Common;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Foundation.UI.PopUp
{
    public class UIConfirmPopUp : UIPopUpPanel
    {
        public Button[] ownButtons;

        private void OnEnable()
        {
            titleText.text = "스테이지 종료";
        }

        public async Task<bool> GetResult()
        {
            var button = await UIButtonAsync.SelectButton<Button>(ownButtons);
            if ("ConfirmButton" == button.name)
            {
                return true;
            }
            else if ("CancelButton" == button.name)
            {
                return false;
            }

            return false;
        }
    }
}