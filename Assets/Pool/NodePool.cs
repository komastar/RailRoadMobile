using UnityEngine;

public class NodePool : ActorPool<NodeObject>
{
    public override NodeObject CreateInstance(int key, Vector3 position, Quaternion rotation)
    {
        throw new System.NotImplementedException();
    }

    public override void LoadPrefabs()
    {
        throw new System.NotImplementedException();
    }
}