using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    public Dictionary<string, Sprite> routeSprites;

    private void Awake()
    {
        routeSprites = new Dictionary<string, Sprite>();
        var railRoadSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Tile/RailRoadSprites.psd").OfType<Sprite>().ToList();
        for (int i = 0; i < railRoadSprites.Count; i++)
        {
            routeSprites.Add(railRoadSprites[i].name, railRoadSprites[i]);
        }

        var railRoadSprites2 = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Tile/RailRoadSprites2.psd").OfType<Sprite>().ToList();
        for (int i = 0; i < railRoadSprites2.Count; i++)
        {
            routeSprites.Add(railRoadSprites2[i].name, railRoadSprites2[i]);
        }
    }
}