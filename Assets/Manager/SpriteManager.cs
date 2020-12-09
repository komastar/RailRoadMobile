using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        public Dictionary<string, Sprite> Sprites { get; private set; }

        private void Awake()
        {
            Sprites = new Dictionary<string, Sprite>();
            var railRoadSprites = Resources.LoadAll<Sprite>("Sprites/Routes").ToList();
            for (int i = 0; i < railRoadSprites.Count; i++)
            {
                Sprites.Add(railRoadSprites[i].name, railRoadSprites[i]);
            }

            var terrainSprites = Resources.LoadAll<Sprite>("Sprites/Terrain").ToList();
            for (int i = 0; i < terrainSprites.Count; i++)
            {
                Sprites.Add(terrainSprites[i].name, terrainSprites[i]);
            }
        }

        public Sprite GetSprite(string key)
        {
            if (Sprites.ContainsKey(key))
            {
                return Sprites[key];
            }
            else
            {
                return null;
            }
        }
    }
}