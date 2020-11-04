using Assets.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Manager
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        private Dictionary<WayType, Sprite> waySprites;

        internal void Init()
        {
            waySprites = new Dictionary<WayType, Sprite>();
            var railRoadSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Tile/RailRoadSprites.psd").OfType<Sprite>().ToList();
            for (int i = 0; i < railRoadSprites.Count; i++)
            {
                waySprites.Add((WayType)Enum.Parse(typeof(WayType), railRoadSprites[i].name), railRoadSprites[i]);
            }

            var railRoadSprites2 = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Tile/RailRoadSprites2.psd").OfType<Sprite>().ToList();
            for (int i = 0; i < railRoadSprites2.Count; i++)
            {
                waySprites.Add((WayType)Enum.Parse(typeof(WayType), railRoadSprites2[i].name), railRoadSprites2[i]);
            }
        }

        internal Sprite GetWaySprite(WayType wayType)
        {
            return waySprites[wayType];
        }
    }
}
