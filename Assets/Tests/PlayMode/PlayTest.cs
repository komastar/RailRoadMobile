using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Manager;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.Play
{
    public class PlayTest
    {
        private ChapterModel chapter;
        private StageModel stage;
        private MapModel map;

        [SetUp]
        public void Setup()
        {
            chapter = DataManager.Get().ChapterData[1];
            GameManager.Get().CurrentChapter = chapter;
            var stageJsonText = Resources.Load<TextAsset>("Data/Stage/Stage01");
            stage = JObject.Parse(stageJsonText.text).ToObject<StageModel>();
            var mapJsonText = Resources.Load<TextAsset>($"Data/Map/{stage.MapName}");
            map = JObject.Parse(mapJsonText.text).ToObject<MapModel>();
            GameManager.Get().CurrentStage = stage;
            GameManager.Get().GameRoom = GameRoomModel.GetSoloPlay();
            SceneManager.LoadScene("GameScene");
        }

        [UnityTest]
        public IEnumerator MapTestSimple()
        {
            var rotateButton = FindObject<Button>("Canvas/CommandPanel/RotateButton");
            var flipButton = FindObject<Button>("Canvas/CommandPanel/FlipButton");
            var fixButton = FindObject<Button>("Canvas/CommandPanel/FixButton");
            var mapObject = FindObject<MapObject>();
            var handObject = FindObject<HandObject>();

            mapObject.MakeMap(map);
            mapObject.OpenMap();

            handObject.stage = stage;
            handObject.Ready();
            handObject.Roll();
            Assert.AreEqual(3, handObject.dices.Count);

            Vector2Int[] position1 = new Vector2Int[3]
            {
                new Vector2Int(1, 2),
                new Vector2Int(2, 3),
                new Vector2Int(1, 3),
            };
            int[] flip1 = new int[3] { 0, 0, 1 };
            int[] rotate1 = new int[3] { 0, 0, 3 };

            Vector2Int[] position2 = new Vector2Int[3]
            {
                new Vector2Int(3, 2),
                new Vector2Int(2, 1),
                new Vector2Int(3, 1),
            };
            int[] flip2 = new int[3] { 0, 0, 1 };
            int[] rotate2 = new int[3] { 2, 2, 1 };

            Vector2Int[][] positions = new Vector2Int[2][];
            positions[0] = position1;
            positions[1] = position2;
            int[][] flips = new int[2][];
            flips[0] = flip1;
            flips[1] = flip2;
            int[][] rotates = new int[2][];
            rotates[0] = rotate1;
            rotates[1] = rotate2;

            for (int i = 0; i < 2; i++)
            {
                var dices = handObject.dices;

                for (int j = 0; j < dices.Count; j++)
                {
                    handObject.Dice = dices[j];

                    var node = mapObject.GetNode(positions[i][j]);
                    mapObject.OnClickNode(node);
                    for (int flip = 0; flip < flips[i][j]; flip++)
                    {
                        flipButton.onClick.Invoke();
                    }

                    for (int rotate = 0; rotate < rotates[i][j]; rotate++)
                    {
                        rotateButton.onClick.Invoke();
                    }

                    fixButton.onClick.Invoke();
                }

                handObject.Roll();

                yield return null;
            }

            yield return new WaitForSecondsRealtime(.5f);

            ScoreViewModel score = new ScoreViewModel();
            mapObject.GetScore(score);
            Assert.AreEqual(12, score.TotalScore);
        }

        private T FindObject<T>(string name = null) where T : Component
        {
            T find;
            if (string.IsNullOrEmpty(name))
            {
                find = Object.FindObjectOfType<T>();
            }
            else
            {
                find = GameObject.Find(name).GetComponent<T>();
            }

            Assert.AreNotEqual(null, find);
            return find;
        }
    }
}
