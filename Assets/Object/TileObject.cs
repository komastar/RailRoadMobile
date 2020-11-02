using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Assets.Object
{
    public class TileObject : MonoBehaviour, IPointerClickHandler
    {
        public SpriteRenderer tileRenderer;
        public Action<TileObject> onClick;

        private void OnDisable()
        {
            onClick = null;
        }

        #region EventHandler
        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this);
        }
        #endregion

        public void SetSprite(Sprite sprite)
        {
            tileRenderer.sprite = sprite;
            name = sprite.name;
        }

        public void Build()
        {
            onClick = null;
        }

        internal void Unnominate()
        {
            transform.localScale = Vector3.one;
        }

        internal void Nominate()
        {
            transform.localScale = Vector3.one * 2f;
        }
    }
}