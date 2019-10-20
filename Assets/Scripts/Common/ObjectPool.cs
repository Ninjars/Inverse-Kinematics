using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool {
    private GameObject pooledObjectPrefab;
    private List<GameObject> pooledObjects;

    public ObjectPool(GameObject prefab, int initialPoolSize) {
        pooledObjectPrefab = prefab;
        pooledObjects = new List<GameObject>(initialPoolSize);
        for (int i = 0; i < initialPoolSize; i++) {
            createNewInstance();
        }
    }

    private GameObject createNewInstance() {
        var obj = GameObject.Instantiate(pooledObjectPrefab);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public GameObject getObject() {
        var obj = pooledObjects.FirstOrDefault(arg => !arg.activeInHierarchy);
        if (obj == null) {
            obj = createNewInstance();
        }
        return obj;
    }
}
