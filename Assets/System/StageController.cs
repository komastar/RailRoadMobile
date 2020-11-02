using Assets.Object;
using Assets.Pool;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.System
{
    public class StageController : MonoBehaviour
    {
        private CancellationTokenSource tokenSource;

        public float tileSize;
        public TileObject tilePrefab;
        public HandObject hand;
        public Vector2Int mapSize;
        public Transform map;
        public Sprite[] tiles;
        public Dictionary<string, Sprite> tileData;
        public TileObject candidateTile;

        private void Awake()
        {
            ObjectPool.tilePrefab = tilePrefab;

            tokenSource = new CancellationTokenSource();
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

        private async void Start()
        {
            await PhaseState.UpdateLoopAsync(this, tokenSource.Token);
        }

        private void OnApplicationQuit()
        {
            tokenSource.Cancel();
        }

        internal async Task Draw()
        {
            hand.ReadyHand();

            for (int i = 0; i < hand.handTransform.Length; i++)
            {
                if (!tokenSource.IsCancellationRequested)
                {
                    MakeTile();

                    await Task.Delay(1000);
                }
            }
        }

        private void MakeTile()
        {
            var newTile = ObjectPool.GetTile();
            int randomIndex = Random.Range(0, tiles.Length - 1);
            newTile.SetSprite(tileData[tiles[randomIndex].name]);
            newTile.onClick += ClickHand;
            hand.Push(newTile);
        }

        private void ClickHand(TileObject clickedTile)
        {
            if (candidateTile == clickedTile)
            {
                candidateTile.Unnominate();
                candidateTile = null;
            }
            else
            {
                if (!ReferenceEquals(null, candidateTile))
                {
                    candidateTile.Unnominate();
                }
                candidateTile = clickedTile;
                candidateTile.Nominate();
            }
        }

        internal void ResetHand()
        {
            candidateTile = null;
            hand.ResetHand();
        }
    }
}