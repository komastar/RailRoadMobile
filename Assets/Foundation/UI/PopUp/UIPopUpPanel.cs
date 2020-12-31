using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Foundation.UI.PopUp
{
    public class UIPopUpPanel : MonoBehaviour
    {
        public Text titleText;

        public UIPopUpPanel[] contents;

        private void Awake()
        {
            if (null == contents)
            {
                TurnOff();
                Init();
            }
        }

        public void Init()
        {
            contents = GetComponentsInChildren<UIPopUpPanel>();
        }

        public UIPopUpPanel Open(string name)
        {
            TurnOn();
            UIPopUpPanel select = null;
            for (int i = 0; i < contents.Length; i++)
            {
                if (name == contents[i].name)
                {
                    select = contents[i];
                }
                else
                {
                    if (this != contents[i])
                    {
                        contents[i].gameObject.SetActive(false);
                    }
                }
            }
            select.TurnOn();

            return select;
        }

        public void TurnOn()
        {
            gameObject.SetActive(true);
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}