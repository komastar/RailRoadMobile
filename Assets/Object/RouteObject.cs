using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Object
{
    public class RouteObject : IRoute
    {
        public NodeObject node;
        public RouteObject[] link;
    }
}
