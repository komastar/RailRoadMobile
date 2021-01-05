using Assets.Foundation.Interface;
using Assets.Foundation.UI.Common;
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
        public static bool IsOn = false;
        public static NodeObject clickedObject;

        public Button[] tutorialIndicators;
        public RectTransform cancelButtonRect;
        public RectTransform rotateButtonRect;
        public RectTransform flipButtonRect;
        public RectTransform roundButtonRect;

        public Action onTutorialDone;

        public UIScreenMaskObject screenMaskObj;
        public MapObject mapObj;
        private ITutorialPhase current;
        private Queue<ITutorialPhase> tutorials;

        private float prevTimeScale;

        private void Awake()
        {
            tutorials = new Queue<ITutorialPhase>();
        }

        private async void Start()
        {
            var pickTutorialData = new TutorialInfo(1001, "화면 상단의 타일을 선택하세요.");
            var buildTutorialData = new TutorialInfo(100017, "경로를 이곳에 배치하세요");
            var rotateTutorialData = new TutorialInfo(-1, "회전 버튼으로 타일을 회전 시킬 수 있습니다.");
            var flipTutorialData = new TutorialInfo(-1, "반전 버튼으로 타일을 반전 시킬 수 있습니다.");
            var removeTutorialData = new TutorialInfo(-1, "두번 탭하여 타일 배치를 취소 할 수 있습니다.");
            var confirmTutorialData = new TutorialInfo(-1, "체크 버튼을 눌러 라운드를 종료합니다.");

            tutorials.Enqueue(new PickDicePhase(this, pickTutorialData));
            tutorials.Enqueue(new BuildRoutePhase(this, buildTutorialData));
            tutorials.Enqueue(new CancelRoutePhase(this, removeTutorialData));
            removeTutorialData.Id = 0;
            tutorials.Enqueue(new CancelRoutePhase(this, removeTutorialData));
            pickTutorialData.Text = "다시 타일을 선택하세요.";
            tutorials.Enqueue(new PickDicePhase(this, pickTutorialData));
            tutorials.Enqueue(new BuildRoutePhase(this, buildTutorialData));
            tutorials.Enqueue(new ConfirmPhase(this, confirmTutorialData));
            pickTutorialData.Id = 1002;

            tutorials.Enqueue(new PickDicePhase(this, pickTutorialData));
            buildTutorialData.Id = 100013;
            tutorials.Enqueue(new BuildRoutePhase(this, buildTutorialData));
            tutorials.Enqueue(new RotateRoutePhase(this, rotateTutorialData));
            tutorials.Enqueue(new ConfirmPhase(this, confirmTutorialData));

            pickTutorialData.Id = 2011;
            tutorials.Enqueue(new PickDicePhase(this, pickTutorialData));
            buildTutorialData.Id = 100012;
            tutorials.Enqueue(new BuildRoutePhase(this, buildTutorialData));
            tutorials.Enqueue(new FlipRoutePhase(this, flipTutorialData));
            tutorials.Enqueue(new ConfirmPhase(this, confirmTutorialData));
            tutorials.Enqueue(new EndPhase(this, new TutorialInfo()));

            ApplyTutorialMode();
            screenMaskObj.onDisable += () =>
            {
                Time.timeScale = prevTimeScale;
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
            IsOn = false;
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

        public void ResetRect()
        {
            screenMaskObj.ResetRect();
        }

        public void ResetRect(int index)
        {
            screenMaskObj.ResetRect(index);
        }
    }

    public abstract class TutorialPhase : ITutorialPhase
    {
        protected GameTutorialObject TutorialObj;
        protected DiceObject Dice;
        protected TutorialInfo TutorialData;

        public TutorialPhase(GameTutorialObject tutorial, TutorialInfo tutorialData)
        {
            TutorialObj = tutorial;
            TutorialData = tutorialData;
        }

        public abstract void Update();
        public void ResetListener()
        {
            TutorialObj.screenMaskObj.onClickActiveArea = null;
        }

        public void AddListener(Action action)
        {
            TutorialObj.screenMaskObj.onClickActiveArea += action;
        }

        public abstract void Enter();
        public abstract void Exit();
    }

    public class PickDicePhase : TutorialPhase
    {
        public PickDicePhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData)
        {
            TutorialObj.screenMaskObj.SetAlpha(.5f);
        }

        public override void Enter()
        {
            TutorialObj.ResetRect();
            Dice = UnityEngine.Object.FindObjectsOfType<DiceObject>().Single();
            Dice.Roll(TutorialData.Id);
            AddListener(() =>
            {
                Dice.Roll(TutorialData.Id);
                Dice.diceButton.onClick?.Invoke();
                Dice.OnClickDice();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            var diceRect = Dice.GetComponent<RectTransform>();
            TutorialObj.SetRect(diceRect.position, diceRect.rect.size, 0);
            TutorialObj.SetText(TutorialData.Text);
        }
    }

    public class BuildRoutePhase : TutorialPhase
    {
        private NodeObject node;
        public BuildRoutePhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                TutorialObj.mapObj.OnClickNode(node);
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            node = UnityEngine.Object.FindObjectsOfType<NodeObject>().Single(n => n.Id == TutorialData.Id);
            GameTutorialObject.clickedObject = node;
            TutorialObj.SetRect(node.GetColliderCenter(), node.GetColliderSize(), 1);
            TutorialObj.SetText(TutorialData.Text);
        }
    }

    public class RotateRoutePhase : TutorialPhase
    {
        public RotateRoutePhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                TutorialObj.mapObj.Rotate();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            TutorialObj.SetText(TutorialData.Text);
            TutorialObj.SetRect(TutorialObj.rotateButtonRect.position, TutorialObj.rotateButtonRect.sizeDelta, 2);
        }
    }

    public class FlipRoutePhase : TutorialPhase
    {
        public FlipRoutePhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                TutorialObj.mapObj.Flip();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            TutorialObj.SetText(TutorialData.Text);
            TutorialObj.SetRect(TutorialObj.flipButtonRect.position, TutorialObj.flipButtonRect.sizeDelta, 2);
        }
    }

    public class CancelRoutePhase : TutorialPhase
    {
        public CancelRoutePhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }

        public override void Enter()
        {
            TutorialObj.ResetRect(2);
            if (0 == TutorialData.Id)
            {
                AddListener(() =>
                {
                    var node = GameTutorialObject.clickedObject;
                    node.ResetNode();
                });
            }
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            TutorialObj.SetText(TutorialData.Text);
        }
    }

    public class ConfirmPhase : TutorialPhase
    {
        public ConfirmPhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }

        public override void Enter()
        {
            AddListener(() =>
            {
                TutorialObj.roundButtonRect.GetComponent<Button>().onClick.Invoke();
            });
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
            var roundRect = TutorialObj.roundButtonRect;
            TutorialObj.SetRect(roundRect.position, roundRect.sizeDelta, 2);
            TutorialObj.SetText(TutorialData.Text);
        }
    }

    public class EndPhase : TutorialPhase
    {
        public EndPhase(GameTutorialObject tutorial, TutorialInfo tutorialData) : base(tutorial, tutorialData) { }
        public override void Enter()
        {
            TutorialObj.onTutorialDone = null;
            TutorialObj.enabled = false;
            TutorialObj.screenMaskObj.TurnOff();
        }

        public override void Exit()
        {
            ResetListener();
        }

        public override void Update()
        {
        }
    }

    public struct TutorialInfo
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public TutorialInfo(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}