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
        var railRoadSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/RailRoadSprites.psd").OfType<Sprite>().ToList();
        for (int i = 0; i < railRoadSprites.Count; i++)
        {
            routeSprites.Add(railRoadSprites[i].name, railRoadSprites[i]);
        }
    }
}