using Komastar.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Komastar.System
{
    public class StageController : MonoBehaviour
    {
        public float tileSize;
        public TileObject tilePrefab;
        public Vector2Int mapSize;
        public Transform map;
        public Sprite[] tiles;
        public Dictionary<string, Sprite> tileData;

        private void Awake()
        {
            tileData = new Dictionary<string, Sprite>();
            for (int i = 0; i < tiles.Length; i++)
            {
                tileData.Add(tiles[i].name, tiles[i]);
            }

            if (mapSize == Vector2.zero)
            {
                mapSize = new Vector2Int(3, 3);
            }

            map.position = new Vector3(map.position.x - (mapSize.x * 0.5f) + tileSize * 0.5f, map.position.y - (mapSize.y * 0.5f) + tileSize * 0.5f);
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    var tile = Instantiate(tilePrefab, map);
                    tile.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 0);
                }
            }
        }
    }
}