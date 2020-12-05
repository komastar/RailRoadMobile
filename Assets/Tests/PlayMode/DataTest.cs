using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class DataTest
    {
        [UnityTest]
        public IEnumerator DataTestChapterStage()
        {
            var dataManager = DataManager.Get();
            var chapter = dataManager.GetFirstChapter();
            Assert.AreEqual(1, chapter.Id);
            var stage = dataManager.GetFirstStage(chapter);
            Assert.AreEqual(10001, stage.Id);
            var nextStage = dataManager.GetNextStage(chapter, stage);
            Assert.AreEqual(10002, nextStage.Id);

            yield return null;
        }
    }
}
