using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class SampleMapTest
    {
        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("GameScene");
        }

        [UnityTest]
        public IEnumerator SampleMapTestWithEnumeratorPasses()
        {
            var mainGame = FindObject<MainGameScene>("SceneScript");
            var rotateButton = FindObject<Button>("Canvas/CommandPanel/RotateButton");
            var flipButton = FindObject<Button>("Canvas/CommandPanel/FlipButton");
            var fixButton = FindObject<Button>("Canvas/CommandPanel/FixButton");

            var mapObj = Object.FindObjectOfType<MapObject>();
            var handObject = Object.FindObjectOfType<HandObject>();
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

            var wait = new WaitForSecondsRealtime(.0f);
            var wait2 = new WaitForSecondsRealtime(.0f);
            for (int i = 0; i < 2; i++)
            {
                var dices = handObject.GetComponentsInChildren<DiceObject>();
                for (int j = 0; j < dices.Length; j++)
                {
                    handObject.Dice = dices[j];

                    var node = mapObj.GetNode(positions[i][j]);
                    mapObj.OnClickNode(node);
                    for (int flip = 0; flip < flips[i][j]; flip++)
                    {
                        flipButton.onClick.Invoke();
                        yield return wait;
                    }

                    for (int rotate = 0; rotate < rotates[i][j]; rotate++)
                    {
                        rotateButton.onClick.Invoke();
                        yield return wait;
                    }

                    fixButton.onClick.Invoke();

                    yield return wait2;
                }

                handObject.Roll();
                yield return wait2;
            }

            yield return wait;

            var scoreObj = FindObject<ScoreObject>();
            Assert.AreEqual(8, scoreObj.Score);
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
