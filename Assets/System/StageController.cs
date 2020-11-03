using Assets.Constant;
using Assets.Object;
using Assets.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.System
{
    public class StageController : MonoBehaviour
    {
        private CancellationTokenSource tokenSource;

        public WayType wayType;
        public float tileSize;
        public TileObject tilePrefab;
        public HandObject hand;
        public Vector2Int mapSize;
        public Transform map;
        public Sprite[] tiles;
        public Dictionary<WayType, Sprite> tileData;
        public TileObject candidateTile;

        private void Awake()
        {
            wayType = WayType.Count;
            ObjectPool.tilePrefab = tilePrefab;

            tokenSource = new CancellationTokenSource();
            tileData = new Dictionary<WayType, Sprite>();
            for (int i = 0; i < tiles.Length; i++)
            {
                tileData.Add((WayType)Enum.Parse(typeof(WayType), tiles[i].name), tiles[i]);
            }

            MakeMap();
        }

        private async void Start()
        {
            await PhaseState.UpdateLoopAsync(this, tokenSource.Token);
        }

        private void OnApplicationQuit()
        {
            tokenSource.Cancel();
        }

        private void MakeMap()
        {
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
                    tile.onClickMap += BuildTile;
                }
            }
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
            WayType genType = WayType.Count;
            if (wayType == WayType.Count)
            {
                int randomIndex = Random.Range(1, (int)WayType.Count);
                Debug.Log(randomIndex);
                genType = (WayType)randomIndex;
            }
            newTile.Ready(tileData[genType]);
            newTile.onClickHand += ClickHand;
            hand.Push(newTile);
        }

        private void BuildTile(TileObject clickedTile)
        {
            if (!ReferenceEquals(null, candidateTile))
            {
                ObjectPool.Return(candidateTile);
                clickedTile.Build(candidateTile);
            }
            else
            {
                candidateTile.Unnominate();
                candidateTile = null;
            }
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

        public void OnClickRotateTile()
        {
            if (!ReferenceEquals(null, candidateTile))
            {
                candidateTile.Rotate();
            }
        }

        public void OnClickFlipTile()
        {
            if (!ReferenceEquals(null, candidateTile))
            {
                candidateTile.Flip();
            }
        }

        public void OnClickConfirm()
        {

        }
    }
}