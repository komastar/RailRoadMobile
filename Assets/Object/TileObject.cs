using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Assets.Constant;

namespace Assets.Object
{
    public class TileObject : MonoBehaviour, IPointerClickHandler
    {
        public int directionIndex;

        public SpriteRenderer tileRenderer;
        public SpriteRenderer wayRenderer;
        public Action<TileObject> onClickHand;
        public Action<TileObject> onClickMap;
        public Action<TileObject> onClickBuild;

        public TileState state;

        #region Unity Methods
        private void Awake()
        {
            directionIndex = 0;
            state = TileState.Empty;
            wayRenderer.sprite = null;
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
        internal void Ready(Sprite sprite)
        {
            onClickMap = null;
            state = TileState.Ready;
            wayRenderer.sprite = sprite;
            name = sprite.name;
        }

        internal void Build(TileObject tile)
        {
            onClickHand = null;
            state = TileState.Build;
            wayRenderer.sprite = tile.GetSprite();
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
            return wayRenderer.sprite;
        }

        internal void Rotate()
        {
            directionIndex++;
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
        #endregion
    }
}