using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    public Dictionary<string, Sprite> RouteSprites { get; private set; }

    private void Awake()
    {
        RouteSprites = new Dictionary<string, Sprite>();
        var railRoadSprites = Resources.LoadAll<Sprite>("Sprites/RailRoadSprites").ToList();
        for (int i = 0; i < railRoadSprites.Count; i++)
        {
            RouteSprites.Add(railRoadSprites[i].name, railRoadSprites[i]);
        }
    }

    public Sprite GetSprite(string key)
    {
        if (RouteSprites.ContainsKey(key))
        {
            return RouteSprites[key];
        }
        else
        {
            return null;
        }
    }
}