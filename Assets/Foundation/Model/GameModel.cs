using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace Assets.Foundation.Model
{
    public partial class GameModel
    {
        public int Id { get; set; }
        public string GameCode { get; set; }
        public int UserCount { get; set; }
        public int MaxUserCount { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreationTime { get; set; }

        public GameModel() { }

        public GameModel(string gamecode, int maxUserCount, bool isOpen = false)
        {
            maxUserCount = maxUserCount > 6 ? 6 : maxUserCount;
            GameCode = gamecode;
            UserCount = 0;
            MaxUserCount = maxUserCount < 2 ? 2 : maxUserCount;
            IsOpen = isOpen;
            CreationTime = DateTime.Now;
        }

        public void Validate()
        {
            MaxUserCount = Mathf.Clamp(MaxUserCount, 2, 6);
            CreationTime = DateTime.Now;
            UserCount = 0;
        }

        public static GameModel GetSoloPlay()
        {
            return new GameModel()
            {
                GameCode = Constant.GameCode.SoloPlay
            };
        }
    }

    public partial class GameModel
    {
        public static GameModel Parse(string json)
        {
            return JObject.Parse(json).ToObject<GameModel>();
        }
    }
}
