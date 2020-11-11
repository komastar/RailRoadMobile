using UnityEngine;

namespace Assets.Scene
{
    public class MainGameScene : MonoBehaviour
    {
        private StageController stage;
        private DiceController dice;

        public WayType wayType;

        private void Awake()
        {
            stage = new StageController();
            dice = new DiceController();
        }
    }
}
