using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Assets.Constant;
using Assets.Manager;

namespace Assets.Object
{
    public class TileObject : MonoBehaviour, IPointerClickHandler, ITile
    {
        public int directionIndex;

        public RouteObject route;
        public SpriteRenderer tileRenderer;
        public SpriteRenderer routeRenderer;
        public Action<TileObject> onClickHand;
        public Action<TileObject> onClickMap;
        public Action<TileObject> onClickBuild;
        public Vector2Int gridPos;

        public TileState state;

        public IRoute Route => throw new NotImplementedException();

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
        internal void Setup(int id)
        {

        }

        internal void Empty()
        {
            directionIndex = 0;
            state = TileState.Empty;
            routeRenderer.sprite = null;
        }

        internal void Ready(WayType wayType)
        {
            onClickMap = null;
            state = TileState.Ready;
            routeRenderer.sprite = SpriteManager.Get().routeSprites[wayType.ToString()];
            name = wayType.ToString();
        }

        internal void Build(TileObject tile)
        {
            name = tile.name;
            onClickHand = null;
            state = TileState.Build;
            routeRenderer.sprite = tile.GetSprite();
            directionIndex = tile.directionIndex;
            UpdateRotation();
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
            return routeRenderer.sprite;
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
            routeRenderer.transform.rotation = Quaternion.Euler(rotation);
        }

        internal void Flip()
        {
            Vector3 scale = routeRenderer.transform.localScale;
            scale.x *= -1f;
            routeRenderer.transform.localScale = scale;
        }

        internal bool IsBuildable(TileObject candidate, MapObject map)
        {
            return true;
        }

        internal JointObject GetJointByDirection(int dirIndex)
        {
            return null;
        }
        #endregion
    }
}