using Assets.Object;
using System.Collections.Generic;

namespace Assets.Pool
{
    public static class ObjectPool
    {
        private static Queue<TileObject> pool;
        public static TileObject tilePrefab;

        public static TileObject GetTile()
        {
            if (pool == null)
            {
                pool = new Queue<TileObject>();
            }

            if (pool.Count > 0)
            {
                var tile = pool.Dequeue();
                tile.gameObject.SetActive(true);

                return tile;
            }
            else
            {
                return UnityEngine.Object.Instantiate(tilePrefab);
            }
        }

        public static void Return(TileObject tile)
        {
            tile.gameObject.SetActive(false);
            pool.Enqueue(tile);
        }
    }
}
