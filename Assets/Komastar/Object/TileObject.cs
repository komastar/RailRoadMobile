using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Komastar.Constant;

namespace Komastar.Object
{
    public class TileObject : MonoBehaviour
    {
        public SpriteRenderer tileRenderer;

        public void SetSprite(Sprite sprite)
        {
            tileRenderer.sprite = sprite;
        }
    }
}