using Assets.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Object
{
    public class WayObject : MonoBehaviour
    {
        public WayType wayType;
        public JointObject[] joints;
        public bool isTransferPossible;

        public void Setup(WayType _wayType)
        {
            isTransferPossible = true;
            wayType = _wayType;
            for (int i = 0; i < 4; i++)
            {
                joints[i].jointType = JointType.None;
            }

            switch (wayType)
            {
                case WayType.Rail_I:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Rail;
                    break;
                case WayType.Rail_L:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Left].jointType = JointType.Rail;
                    break;
                case WayType.Rail_T:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Left].jointType = JointType.Rail;
                    joints[Way.Right].jointType = JointType.Rail;
                    break;
                case WayType.Rail_T_To_Road_I:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Rail;
                    joints[Way.Right].jointType = JointType.Rail;
                    break;
                case WayType.Rail_X:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Left].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Rail;
                    joints[Way.Right].jointType = JointType.Rail;
                    break;
                case WayType.RailToRoad_I:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Road;
                    break;
                case WayType.RailToRoad_L:
                    joints[Way.Up].jointType = JointType.Rail;
                    joints[Way.Left].jointType = JointType.Road;
                    break;
                case WayType.Road_And_Rail_X:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Road;
                    joints[Way.Right].jointType = JointType.Rail;
                    isTransferPossible = false;
                    break;
                case WayType.Road_I:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Down].jointType = JointType.Road;
                    break;
                case WayType.Road_L:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Road;
                    break;
                case WayType.Road_L_Rail_L:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Road;
                    joints[Way.Down].jointType = JointType.Rail;
                    joints[Way.Right].jointType = JointType.Rail;
                    break;
                case WayType.Road_T:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Road;
                    joints[Way.Right].jointType = JointType.Road;
                    break;
                case WayType.Road_T_To_Rail_I:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Road;
                    joints[Way.Down].jointType = JointType.Rail;
                    joints[Way.Right].jointType = JointType.Road;
                    break;
                case WayType.Road_X:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Road;
                    joints[Way.Down].jointType = JointType.Road;
                    joints[Way.Right].jointType = JointType.Road;
                    break;
                case WayType.RoadToRail_X:
                    joints[Way.Up].jointType = JointType.Road;
                    joints[Way.Left].jointType = JointType.Rail;
                    joints[Way.Down].jointType = JointType.Road;
                    joints[Way.Right].jointType = JointType.Rail;
                    break;
                case WayType.Rail_Start:
                    joints[Way.Up].jointType = JointType.Rail;
                    break;
                case WayType.Road_Start:
                    joints[Way.Up].jointType = JointType.Road;
                    break;
            }
        }
    }
}
