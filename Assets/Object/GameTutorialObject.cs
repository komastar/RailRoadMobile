using Assets.Foundation.Interface;
using Assets.UI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Object
{
    public class GameTutorialObject : MonoBehaviour
    {
        public Button[] tutorialIndicators;

        public UIScreenMaskObject screenMaskObj;
        public IActor clickedObject;
        private ITutorialPhase current;
        private Queue<ITutorialPhase> tutorials;

        private float prevTimeScale;

        private void Awake()
        {
            tutorials = new Queue<ITutorialPhase>();
        }

        private async void Start()
        {
            tutorials.Enqueue(new PickDicePhase(this));
            tutorials.Enqueue(new BuildRoutePhase(this));
            tutorials.Enqueue(new ConfirmPhase(this));

            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            screenMaskObj.TurnOn();
            await Task.Delay(250);
            while (0 < tutorials.Count && enabled)
            {
                current = tutorials.Dequeue();
                current.Enter();
                current.Update(clickedObject);

                var button = await UIButtonAsync.SelectButton<Button>(tutorialIndicators);
                if (button.name.ToLower().Contains("exit"))
                {
                    enabled = false;
                    screenMaskObj.TurnOff();
                }
                current.Exit();
            }
        }

        private void OnDisable()
        {
            Time.timeScale = prevTimeScale;
        }

        public void OnClickObject(IActor actor)
        {
            clickedObject = actor;
        }
    }

    public abstract class TutorialPhase : ITutorialPhase
    {
        protected GameTutorialObject tutorialObject;
        public TutorialPhase(GameTutorialObject tutorial)
        {
            tutorialObject = tutorial;
        }

        public abstract ITutorialPhase Update(IActor actor);
        public void ResetListener()
        {
            tutorialObject.screenMaskObj.onClickActiveArea = null;
        }

        public void AddListener(Action action)
        {
            tutorialObject.screenMaskObj.onClickActiveArea += action;
        }

        public abstract void Enter();
        public abstract void Exit();
    }

    public class PickDicePhase : TutorialPhase
    {
        private DiceObject dice;
        public PickDicePhase(GameTutorialObject tutorial) : base(tutorial)
        {
            tutorialObject.screenMaskObj.SetAlpha(.1f);
        }

        public override void Enter()
        {
            ResetListener();
            AddListener(() =>
            {
                dice.OnClickDice();
            });
        }

        public override void Exit()
        {
        }

        public override ITutorialPhase Update(IActor actor)
        {
            dice = UnityEngine.Object.FindObjectsOfType<DiceObject>().Single(d => d.Id == 404);
            var diceRect = dice.GetComponent<RectTransform>();
            tutorialObject.screenMaskObj.SetRect(diceRect.position, diceRect.rect.size);
            tutorialObject.screenMaskObj.SetText("화면 상단의 타일을 선택하세요.");

            return this;
        }
    }

    public class BuildRoutePhase : TutorialPhase
    {
        private NodeObject node;
        public BuildRoutePhase(GameTutorialObject tutorial) : base(tutorial)
        {
        }

        public override void Enter()
        {
            ResetListener();
            AddListener(() =>
            {
                node.OnPointerClick(null);
            });
        }

        public override void Exit()
        {
        }

        public override ITutorialPhase Update(IActor actor)
        {
            node = UnityEngine.Object.FindObjectsOfType<NodeObject>().Single(n => n.Id == 100004);
            tutorialObject.screenMaskObj.SetRect(node.GetColliderCenter(), node.GetColliderSize());
            tutorialObject.screenMaskObj.SetText("경로를 이곳에 배치하세요");

            return this;
        }
    }

    public class ConfirmPhase : TutorialPhase
    {
        public ConfirmPhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            ResetListener();
            tutorialObject.enabled = false;
            tutorialObject.screenMaskObj.TurnOff();
        }

        public override void Exit()
        {
        }

        public override ITutorialPhase Update(IActor actor)
        {
            return null;
        }
    }
}