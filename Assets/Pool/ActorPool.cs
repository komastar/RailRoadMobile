using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorPool<T> where T : Component, IActor
{
    protected Dictionary<int, T> _prefabs = null;
    protected Dictionary<int, Queue<T>> _pool = null;
    protected Transform _parent = null;

    public ActorPool()
    {
        _pool = new Dictionary<int, Queue<T>>();
        _prefabs = new Dictionary<int, T>();
        _parent = new GameObject(typeof(T).Name).transform;
        LoadPrefabs();
    }

    public abstract void LoadPrefabs();
    public abstract T CreateInstance(int key, Vector3 position, Quaternion rotation);

    public void Preload(int key, int count)
    {
        List<T> preloadActor = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var actor = Rent(key);
            actor.Id = key;
            preloadActor.Add(actor);
        }

        for (int i = 0; i < preloadActor.Count; i++)
        {
            Return(preloadActor[i]);
        }
    }

    public void ClearPool()
    {
        foreach (var item in _pool)
        {
            while (0 < item.Value.Count)
            {
                T actor = item.Value.Dequeue();
                UnityEngine.Object.Destroy(actor.gameObject);
            }
        }

        _pool.Clear();
    }

    private void MakePool(int key)
    {
        if (false == _pool.ContainsKey(key))
        {
            _pool.Add(key, new Queue<T>());
        }
    }

    public T Rent(int key)
    {
        return Rent(key, Vector3.zero, Quaternion.identity);
    }

    public T Rent(int key, Vector3 position)
    {
        return Rent(key, position, Quaternion.identity);
    }

    public T Rent(int key, Vector3 position, Quaternion rotation)
    {
        if (_prefabs.Count == 0)
        {
            throw new ArgumentNullException("No prefab");
        }

        MakePool(key);

        T actor = _pool[key].Count > 0 ? _pool[key].Dequeue() : CreateInstance(key, position, rotation);
        OnBeforeRent(actor, position, rotation);

        return actor;
    }

    public void Return(T actor)
    {
        Return(actor.Id, actor);
    }

    public void Return(int key, T actor)
    {
        MakePool(actor.Id);

        OnBeforeReturn(actor);
        _pool[key].Enqueue(actor);
    }


    public virtual void OnBeforeRent(T actor, Vector3 position, Quaternion rotation)
    {
        actor.gameObject.transform.position = position;
        actor.gameObject.transform.rotation = rotation;
        actor.gameObject.SetActive(true);
    }

    public virtual void OnBeforeReturn(T actor)
    {
        actor.gameObject.SetActive(false);
        actor.gameObject.transform.position = Vector3.zero;
        actor.gameObject.transform.rotation = Quaternion.identity;
    }
}