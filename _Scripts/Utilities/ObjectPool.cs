using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public struct Pool
    {
        public Queue<GameObject> pooledObjects;
        public GameObject objectPrefab;
        public int poolSize;
    }

    [SerializeField] private Pool[] pools = null;
    private void Awake()
    {
        for (int j = 0; j < pools.Length; j++)
        {
            if(pools[j].objectPrefab == null)
            {
                Debug.LogWarning("Object prefab is null in pool " + j);
                continue;
            }
            pools[j].pooledObjects = new Queue<GameObject>();

            for (int i = 0; i <  pools[j].poolSize; i++)
            {
                GameObject obj = Instantiate(pools[j].objectPrefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
            
                pools[j].pooledObjects.Enqueue(obj);
            }
        }
    }
    public GameObject GetPooledObject(int objectType)
    {
        if (objectType >= pools.Length)
            return null;

        GameObject obj =  pools[objectType].pooledObjects.Dequeue();
        
        obj.SetActive(true);
        
        pools[objectType].pooledObjects.Enqueue(obj);

        return obj;
    }
}
