using Assets.Foundation.Interface;
using Assets.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Object
{
    public class GameTutorialObject : MonoBehaviour
    {
        public static NodeObject clickedObject;

        public Button[] tutorialIndicators;
        public RectTransform cancelButtonRect;
        public RectTransform rotateButtonRect;
        public RectTransform flipButtonRect;
        public RectTransform roundButtonRect;

        public Action onTutorialDone;

        public UIScreenMaskObject screenMaskObj;
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
            tutorials.Enqueue(new RotateRoutePhase(this));
            tutorials.Enqueue(new RotateRoutePhase2(this));
            tutorials.Enqueue(new FlipRoutePhase(this));
            tutorials.Enqueue(new ConfirmPhase(this));
            tutorials.Enqueue(new EndPhase(this));

            ApplyTutorialMode();
            screenMaskObj.onDisable += () =>
            {
                Time.timeScale = prevTimeScale;
                enabled = false;
            };
            screenMaskObj.TurnOn();
            await Task.Delay(250);
            while (0 < tutorials.Count && enabled)
            {
                current = tutorials.Dequeue();
                current.Enter();
                current.Update();

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
            onTutorialDone?.Invoke();
        }

        private void ApplyTutorialMode()
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        public void SetText(string text)
        {
            screenMaskObj.SetText(text);
        }

        public void SetRect(Vector2 position, Vector2 size, int index = 0)
        {
            screenMaskObj.SetRect(position, size, index);
        }
    }

    public abstract class TutorialPhase : ITutorialPhase
    {
        protected GameTutorialObject tutorialObject;
        public TutorialPhase(GameTutorialObject tutorial)
        {
            tutorialObject = tutorial;
        }

        public abstract void Update();
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
            AddListener(() =>
            {
                dice.OnClickDice();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            dice = UnityEngine.Object.FindObjectsOfType<DiceObject>().Single(d => d.Id == 404);
            var diceRect = dice.GetComponent<RectTransform>();
            tutorialObject.SetRect(diceRect.position, diceRect.rect.size);
            tutorialObject.SetText("화면 상단의 타일을 선택하세요.");
        }
    }

    public class BuildRoutePhase : TutorialPhase
    {
        private NodeObject node;
        public BuildRoutePhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                node.OnPointerClick(null);
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            node = UnityEngine.Object.FindObjectsOfType<NodeObject>().Single(n => n.Id == 100004);
            GameTutorialObject.clickedObject = node;
            tutorialObject.SetRect(node.GetColliderCenter(), node.GetColliderSize());
            tutorialObject.SetText("경로를 이곳에 배치하세요");
        }
    }

    public class RotateRoutePhase : TutorialPhase
    {
        public RotateRoutePhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                var node = GameTutorialObject.clickedObject;
                node.Rotate();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            tutorialObject.SetText("회전 버튼으로 타일을 회전 시킬 수 있습니다.");
            tutorialObject.SetRect(tutorialObject.rotateButtonRect.position, tutorialObject.rotateButtonRect.sizeDelta, 1);
        }
    }

    public class RotateRoutePhase2 : TutorialPhase
    {
        public RotateRoutePhase2(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                var node = GameTutorialObject.clickedObject;
                node.Rotate();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            tutorialObject.SetText("한번 더 회전 시켜보세요.");
        }
    }

    public class FlipRoutePhase : TutorialPhase
    {
        public FlipRoutePhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            tutorialObject.SetText(
                "일부 타일은 반전이 가능합니다.\n" +
                "반전 버튼으로 타일을 반전 시킬 수 있습니다.\n");
            tutorialObject.screenMaskObj.SetRect(tutorialObject.flipButtonRect.position, tutorialObject.flipButtonRect.sizeDelta, 1);
        }
    }

    public class ConfirmPhase : TutorialPhase
    {
        public ConfirmPhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            ResetListener();
            AddListener(() =>
            {
                tutorialObject.roundButtonRect.GetComponent<Button>().onClick.Invoke();
            });
            tutorialObject.screenMaskObj.ResetRect(1);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            var roundRect = tutorialObject.roundButtonRect;
            tutorialObject.SetRect(roundRect.position, roundRect.sizeDelta);
            tutorialObject.SetText("배치를 완료했으면 완료 버튼을 누르세요");
        }
    }

    public class EndPhase : TutorialPhase
    {
        public EndPhase(GameTutorialObject tutorial) : base(tutorial) { }

        public override void Enter()
        {
            tutorialObject.onTutorialDone = null;
            tutorialObject.enabled = false;
            tutorialObject.screenMaskObj.TurnOff();
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
        }
    }
}