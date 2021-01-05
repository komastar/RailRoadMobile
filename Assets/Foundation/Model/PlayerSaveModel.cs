using System;
using System.Collections.Generic;

namespace Assets.Foundation.Model
{
    public class PlayerSaveModel
    {
        public int Version { get; set; }
        public List<StageClearModel> ClearStages { get; set; }
        public int RewardAdViewCount { get; set; }
        public DateTime LastRewardAdViewTime { get; set; }

        public PlayerSaveModel()
        {
            ClearStages = new List<StageClearModel>();
        }

        public PlayerSaveModel(int version)
        {
            Version = version;
            ClearStages = new List<StageClearModel>();
        }

        public static PlayerSaveModel MakeNewPlayerData()
        {
            return new PlayerSaveModel(1);
        }

        public bool IsClearStage(int stageId)
        {
            var find = ClearStages.Find(s => s.Id == stageId);
            if (null == find)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ClearStage(int stageId)
        {
            if (!IsClearStage(stageId))
            {
                ClearStages.Add(new StageClearModel() { Id = stageId });
            }
        }
    }

    public class StageClearModel
    {
        public int Id { get; set; }
    }
}
