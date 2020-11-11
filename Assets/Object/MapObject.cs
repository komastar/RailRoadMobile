using Assets.Constant;
using Assets.Manager;
using Assets.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Object
{
    public class MapObject : MonoBehaviour
    {
        public float tileSize;
        public TileObject[][] map;
        public Vector2Int mapSize;
        public HashSet<TileObject> openTile;
        public HashSet<TileObject> closedTile;

        private void Awake()
        {
            openTile = new HashSet<TileObject>();
        }

        public void MakeMap(MapModel mapData)
        {
            mapSize = new Vector2Int(mapData.MapSize.X, mapData.MapSize.Y);
            TileObject emptyTile = ObjectPool.GetEmptyTile();
        }

        public void MakeMap(Action<TileObject> onClickMap)
        {
            if (mapSize == Vector2Int.zero)
            {
                mapSize = new Vector2Int(5, 5);
            }

            map = new TileObject[mapSize.x][];
            for (int x = 0; x < mapSize.x; x++)
            {
                map[x] = new TileObject[mapSize.y];
            }

            transform.position = new Vector3(transform.position.x - (mapSize.x * 0.5f) + tileSize * 0.5f, transform.position.y - (mapSize.y * 0.5f) + tileSize * 0.5f);
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    var tile = ObjectPool.GetEmptyTile();
                    tile.transform.parent = transform;
                    tile.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 0);
                    tile.Empty();
                    tile.onClickMap += onClickMap;
                    tile.gridPos = new Vector2Int(x, y);
                    map[x][y] = tile;
                }
            }

            MakeStartTile(map[2][0], WayType.Rail_Start);
            MakeStartTile(map[2][4], WayType.Rail_Start, 2);

            MakeStartTile(map[0][2], WayType.Road_Start, 3);
            MakeStartTile(map[4][2], WayType.Road_Start, 1);
        }

        private void MakeStartTile(TileObject tile, WayType wayType, int rotate = 0)
        {
            tile.Ready(wayType);
            tile.state = TileState.Fix;
            if (rotate > 0)
            {
                tile.Rotate(rotate);
            }
        }

        internal bool IsBuildable(TileObject clickedTile)
        {
            return openTile.Contains(clickedTile);
        }

        public TileObject GetNeighborTile(Vector2Int pos, int dirIndex)
        {
            switch (dirIndex)
            {
                case 0:
                    pos += Vector2Int.up;
                    break;
                case 1:
                    pos += Vector2Int.left;
                    break;
                case 2:
                    pos += Vector2Int.down;
                    break;
                case 3:
                    pos += Vector2Int.right;
                    break;
            }

            if (pos.x < 0 || pos.x > mapSize.x - 1
                || pos.y < 0 || pos.y > mapSize.y - 1)
            {
                return null;
            }

            return map[pos.x][pos.y];
        }
    }
}
