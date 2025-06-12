using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectMemoryPool : MonoBehaviour
{

    /*
    public class Pool : MemoryPool<GameObject>
    {
        public GameObject objectPrefab;
        public int poolSize;
    }*/

    [Serializable]
    public struct Pool
    {
        public GameObject objectPrefab;
        public int poolSize;
    }

    [SerializeField] private Pool[] pools = null;

    private DiContainer _container;
    private Dictionary<int, MemoryPool<GameObject>> _poolsDictionary;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
        _poolsDictionary = new Dictionary<int, MemoryPool<GameObject>>();
    }

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].objectPrefab == null)
            {
                Debug.LogError($"Object prefab is null in pool {i}");
                continue;
            }

            // Create a MemoryPool for each pool definition
            var pool = _container.Instantiate<GameObjectMemoryPool>( new object[] { pools[i].objectPrefab, pools[i].poolSize } );

            _poolsDictionary.Add(i, pool);
        }
    }

    public GameObject GetPooledObject(int objectType)
    {
        if (!_poolsDictionary.ContainsKey(objectType))
        {
            Debug.LogError($"Pool with type {objectType} does not exist.");
            return null;
        }

        // Spawn an object from the pool
        GameObject obj = _poolsDictionary[objectType].Spawn();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnPooledObject(int objectType, GameObject obj)
    {
        if (!_poolsDictionary.ContainsKey(objectType))
        {
            Debug.LogError($"Pool with type {objectType} does not exist.");
            return;
        }

        // Return the object to the pool
        obj.SetActive(false);
        _poolsDictionary[objectType].Despawn(obj);
    }
}

// Custom MemoryPool for GameObjects
public class GameObjectMemoryPool : MemoryPool<GameObject>
{
    private readonly GameObject _prefab;
    private readonly int _poolSize;

    public GameObjectMemoryPool(GameObject prefab, int poolSize)
    {
        _prefab = prefab;
        _poolSize = poolSize;
    }

    protected override void OnCreated(GameObject item)
    {
        // Initialize the object when it's created
        item.SetActive(false);
    }

    protected override void OnSpawned(GameObject item)
    {
        // Activate the object when it's spawned
        item.SetActive(true);
    }

    protected override void OnDespawned(GameObject item)
    {
        // Deactivate the object when it's despawned
        item.SetActive(false);
    }
}