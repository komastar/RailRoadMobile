using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Object
{
    [SelectionBase]
    public class NodeObject : MonoBehaviour
    {
        public int routeId;
        public int direction = 0;
        public NodeObject[] neighbors;
        public GridInt Position;
        public GameObject routeRenderer;

        public void Rotate(int dir = -1)
        {
            direction += (dir == -1 ? 1 : dir);
            direction %= 4;
            UpdateRotation();
        }
        private void UpdateRotation()
        {
            Vector3 rotation = new Vector3(0f, 0f, direction * 90f);
            routeRenderer.transform.rotation = Quaternion.Euler(rotation);
        }

        public void SetupNode(int id)
        {
            routeId = id;
        }

        public void ResetNode()
        {
            routeId = 0;
            direction = 0;
            UpdateRotation();
        }
    }
}
