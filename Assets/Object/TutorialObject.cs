using Assets.Foundation.Interface;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Object
{
    public class TutorialObject : MonoBehaviour
    {
        public UIScreenMaskObject screenMaskObj;
        public IActor clickedObject;
        private ITutorialPhase current;

        private void Start()
        {
            current = new PickDicePhase(this);
        }

        private void LateUpdate()
        {
            if (null != current)
            {
                current = current.Update(clickedObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnClickObject(IActor actor)
        {
            clickedObject = actor;
        }
    }

    public abstract class TutorialPhase : ITutorialPhase
    {
        protected TutorialObject tutorialObject;
        public TutorialPhase(TutorialObject tutorial)
        {
            tutorialObject = tutorial;
        }

        public abstract ITutorialPhase Update(IActor actor);
    }

    public class PickDicePhase : TutorialPhase
    {
        public PickDicePhase(TutorialObject tutorial) : base(tutorial)
        {
            tutorialObject.screenMaskObj.SetAlpha(.1f);
        }

        public override ITutorialPhase Update(IActor actor)
        {
            DiceObject dice = UnityEngine.Object.FindObjectsOfType<DiceObject>().Single(d => d.Id == 404);
            var diceRect = dice.GetComponent<RectTransform>();
            tutorialObject.screenMaskObj.SetRect(diceRect.position, diceRect.rect.size);
            tutorialObject.screenMaskObj.SetText("주어진 주사위를 고르세요");
            tutorialObject.screenMaskObj.TurnOn();

            if (false == ReferenceEquals(null, actor))
            {
                if (404 == actor.Id)
                {
                    return new BuildRoutePhase(tutorialObject);
                }
            }

            return this;
        }
    }

    public class BuildRoutePhase : TutorialPhase
    {
        public BuildRoutePhase(TutorialObject tutorial) : base(tutorial) { }

        public override ITutorialPhase Update(IActor actor)
        {
            NodeObject node = UnityEngine.Object.FindObjectsOfType<NodeObject>().Single(n => n.Id == 100004);
            tutorialObject.screenMaskObj.SetRect(node.GetColliderCenter(), node.GetColliderSize());
            tutorialObject.screenMaskObj.SetText("경로를 이곳에 배치하세요");

            if (false == ReferenceEquals(null, actor))
            {
                if (100004 == actor.Id)
                {
                    return new ConfirmPhase(tutorialObject);
                }
            }

            return this;
        }
    }

    public class ConfirmPhase : TutorialPhase
    {
        public ConfirmPhase(TutorialObject tutorial) : base(tutorial) { }

        public override ITutorialPhase Update(IActor actor)
        {
            tutorialObject.screenMaskObj.TurnOff();

            return null;
        }
    }
}