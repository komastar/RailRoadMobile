using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Assets.Constant;
using Assets.Manager;

namespace Assets.Object
{
    public class TileObject : MonoBehaviour, IPointerClickHandler
    {
        public int directionIndex;

        public WayObject way;
        public SpriteRenderer tileRenderer;
        public SpriteRenderer wayRenderer;
        public Action<TileObject> onClickHand;
        public Action<TileObject> onClickMap;
        public Action<TileObject> onClickBuild;
        public Vector2Int gridPos;

        public TileState state;

        #region Unity Methods
        private void Awake()
        {
            Empty();
        }

        private void OnDisable()
        {
            Unnominate();
            onClickHand = null;
            onClickMap = null;
            onClickBuild = null;
        }
        #endregion

        #region EventHandler
        public void OnPointerClick(PointerEventData eventData)
        {
            switch (state)
            {
                case TileState.Empty:
                    onClickMap?.Invoke(this);
                    break;
                case TileState.Ready:
                    onClickHand?.Invoke(this);
                    break;
                case TileState.Build:
                    onClickBuild?.Invoke(this);
                    break;
                case TileState.Fix:
                    break;
            }
        }
        #endregion

        #region Custom Methods
        internal void Empty()
        {
            directionIndex = 0;
            state = TileState.Empty;
            wayRenderer.sprite = null;
        }

        internal void Ready(WayType wayType)
        {
            onClickMap = null;
            state = TileState.Ready;
            wayRenderer.sprite = SpriteManager.Get().GetWaySprite(wayType);
            name = wayType.ToString();
            way.Setup(wayType);
        }

        internal void Build(TileObject tile)
        {
            name = tile.name;
            onClickHand = null;
            state = TileState.Build;
            wayRenderer.sprite = tile.GetSprite();
            directionIndex = tile.directionIndex;
            UpdateRotation();
            way.wayType = tile.way.wayType;
            for (int i = 0; i < 4; i++)
            {
                way.joints[i].jointType = tile.way.joints[(i + directionIndex) % 4].jointType;
            }
        }

        internal void Fix()
        {
            onClickBuild = null;
            state = TileState.Fix;
        }

        internal void Unnominate()
        {
            transform.localScale = Vector3.one;
        }

        internal void Nominate()
        {
            transform.localScale = Vector3.one * 2f;
        }

        internal Sprite GetSprite()
        {
            return wayRenderer.sprite;
        }

        internal void Rotate(int jump = 1)
        {
            directionIndex += jump;
            directionIndex %= 4;
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            Vector3 rotation = new Vector3(0f, 0f, directionIndex * 90f);
            wayRenderer.transform.rotation = Quaternion.Euler(rotation);
        }

        internal void Flip()
        {
            Vector3 scale = wayRenderer.transform.localScale;
            scale.x *= -1f;
            wayRenderer.transform.localScale = scale;
        }

        internal bool IsBuildable(TileObject candidate, MapObject map)
        {
            int missMatchCount = 0;
            int emptyCount = 0;
            for (int i = 0; i < 4; i++)
            {
                int dirIndex = (i + candidate.directionIndex) % 4;
                var joint = candidate.way.joints[i];
                if (joint.jointType != JointType.None)
                {
                    var neighbor = map.GetNeighborTile(gridPos, dirIndex);
                    if (neighbor != null)
                    {
                        if (neighbor.way.wayType == WayType.Empty)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            var neighborType = neighbor.GetJointByDirection((dirIndex + 2) % 4).jointType;
                            if (joint.jointType != neighborType)
                            {
                                return false;
                            }
                            else
                            {
                                //  neighborType == jointType : pass
                            }
                        }
                    }
                }
            }

            if (emptyCount == 4)
            {
                return false;
            }

            return missMatchCount == 0;
        }

        internal JointObject GetJointByDirection(int dirIndex)
        {
            return way.joints[dirIndex];
        }
        #endregion
    }
}