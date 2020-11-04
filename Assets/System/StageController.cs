using Assets.Constant;
using Assets.Manager;
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

        public TileObject tilePrefab;
        public HandObject hand;
        public MapObject map;

        public TileObject candidateTile;

        private void Awake()
        {
            SpriteManager.Get().Init();

            wayType = WayType.Count;
            ObjectPool.tilePrefab = tilePrefab;

            tokenSource = new CancellationTokenSource();

            map.MakeMap(BuildTile);
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
                if (!tokenSource.Token.IsCancellationRequested)
                {
                    MakeTile();

                    await Task.Delay(500);
                }
            }
        }

        private void MakeTile()
        {
            var newTile = ObjectPool.GetTile();
            WayType genType = wayType;
            if (genType == WayType.Count)
            {
                int randomIndex = Random.Range(1, (int)WayType.Count);
                genType = (WayType)randomIndex;
            }
            newTile.Ready(genType);
            newTile.onClickHand += ClickHand;
            hand.Push(newTile);
        }

        private void BuildTile(TileObject clickedTile)
        {
            if (!ReferenceEquals(null, candidateTile))
            {
                if (map.IsBuildable(clickedTile))
                {
                    if (clickedTile.IsBuildable(candidateTile, map))
                    {
                        clickedTile.Build(candidateTile);
                        ObjectPool.Return(candidateTile);
                    }
                }
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