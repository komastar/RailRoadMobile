using Assets.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model
{
    public struct MapModel
    {
        public TileModel[] Start { get; set; }
    }

    public struct TileModel
    {
        public Vector2Int Position { get; set; }
        public Direction Direction { get; set; }
    }
}
