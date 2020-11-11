using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool<T> where T : Component
{
    private static Queue<T> pool;
    public static T prefab;

    public static T GetEmptyTile()
    {
        if (pool == null)
        {
            pool = new Queue<T>();
        }

        if (pool.Count > 0)
        {
            var tile = pool.Dequeue();
            tile.gameObject.SetActive(true);

            return tile;
        }
        else
        {
            return Object.Instantiate(prefab);
        }
    }

    public static void Return(T tile)
    {
        tile.gameObject.SetActive(false);
        pool.Enqueue(tile);
    }
}