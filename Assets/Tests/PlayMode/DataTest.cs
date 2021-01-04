using System.Collections;
using Manager;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.Data
{
    public class DataTest
    {
        [UnityTest]
        public IEnumerator DataTestChapterStage()
        {
            var dataManager = DataManager.Get();
            var chapter = dataManager.GetFirstChapter();
            Assert.AreEqual(0, chapter.Id);
            var stage = dataManager.GetFirstStage(chapter);
            Assert.AreEqual(10000, stage.Id);
            chapter = dataManager.GetNextChapter(chapter);
            stage = dataManager.GetFirstStage(chapter);
            Assert.AreEqual(10001, stage.Id);
            stage = dataManager.GetNextStage(chapter, stage);
            Assert.AreEqual(10002, stage.Id);

            yield return null;
        }
    }
}
