using Assets.Pool;
using System;
using UnityEngine;

namespace Assets.Object
{
    public class HandObject : MonoBehaviour
    {
        private int index;

        public TileObject[] tiles;
        public Transform[] handTransform;

        private void Awake()
        {
            tiles = new TileObject[handTransform.Length];
        }

        public void ReadyHand()
        {
            index = 0;
        }

        internal void ResetHand()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                ObjectPool.Return(tiles[i]);

                tiles[i] = null;
            }
        }

        public void Push(TileObject tile)
        {
            tile.transform.parent = handTransform[index].transform;
            tile.transform.localPosition = Vector3.zero;
            tiles[index] = tile;
            index++;
        }
    }
}
