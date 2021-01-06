using Assets.Foundation.Constant;
using Assets.Foundation.Model;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        private PlayerSaveModel playerSaveData;

        private GameRoomModel gameRoom;
        public GameRoomModel GameRoom
        {
            get
            {
                if (null == gameRoom)
                {
                    gameRoom = GameRoomModel.GetSoloPlay();
                }

                return gameRoom;
            }
            set
            {
                gameRoom = value;
            }
        }
        public string GameUserId { get; set; }

        public ChapterModel CurrentChapter { get; set; }
        public StageModel CurrentStage { get; set; }

        private int lastChapterIndex;

        private void Awake()
        {
            UrlTable.IsRemote = false;
            Init();
        }

        private void OnApplicationQuit()
        {
            SavePlayerData();
        }

        private void Init()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            LoadPlayerData();
        }

        private void LoadPlayerData()
        {
            string savePath = $"{Application.persistentDataPath}/Player.json";
            if (File.Exists(savePath))
            {
                playerSaveData = JObject.Parse(File.ReadAllText(savePath)).ToObject<PlayerSaveModel>();
            }
            else
            {
                playerSaveData = PlayerSaveModel.MakeNewPlayerData();
                SavePlayerData();
            }
        }

        public int GetLastChapterIndex()
        {
            return playerSaveData.LastChapterIndex;
        }

        public void SetLastChapterIndex(int index)
        {
            playerSaveData.LastChapterIndex = index;
        }

        private void SavePlayerData()
        {
            string savePath = $"{Application.persistentDataPath}/Player.json";
            File.WriteAllText(savePath, JObject.FromObject(playerSaveData).ToString(Newtonsoft.Json.Formatting.Indented));
        }

        public void ReportScore(ScoreViewModel score)
        {
            if (IsSoloPlay()
                && 0 < score.TotalScore)
            {
                ClearStage(score.StageId);
            }
        }

        public bool IsSoloPlay()
        {
            return (GameCode.SoloPlay == GameRoom.GameCode);
        }

        public void ClearStage(int stageId)
        {
            playerSaveData.ClearStage(stageId);
            SavePlayerData();
        }

        public bool IsClearStage(int stageId)
        {
            return playerSaveData.IsClearStage(stageId);
        }

        public void AddRewardCount(int count)
        {
            playerSaveData.LastRewardAdViewTime = DateTime.Now.AddHours(12);
            playerSaveData.RewardAdViewCount += count;
        }

        public bool IsRewardAdAvailable()
        {
            return playerSaveData.LastRewardAdViewTime < DateTime.Now;
        }
    }
}