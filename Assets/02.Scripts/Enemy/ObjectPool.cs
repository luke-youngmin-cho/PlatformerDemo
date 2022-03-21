using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;
    public static ObjectPool instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<ObjectPool>("Assets/ObjectPool"));
            return _instance;
        }
    }
    [SerializeField] List<PoolElement> poolElements;
    List<GameObject> spawnedObjects = new List<GameObject>();
    Dictionary<string, Queue<GameObject>> spawnedQueueDictionary = new Dictionary<string, Queue<GameObject>>();


    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public void AddPoolElement(PoolElement poolElement) =>
        poolElements.Add(poolElement);

    public static GameObject SpawnFromPool(string tag, Vector3 position) =>
        instance.Spawn(tag, position);

    public static T SpawnFromPool<T>(string tag, Vector3 position) where T : Component
    {
        GameObject obj = instance.Spawn(tag, position);
        if(obj.TryGetComponent(out T component))
            return component;
        else
        {
            obj.SetActive(false);
            throw new Exception($"Component not found");
        }
    }

    public static void ReturnToPool(GameObject obj)
    {
        if (!instance.spawnedQueueDictionary.ContainsKey(obj.name))
            throw new Exception($"Pool doesn't include {obj.name}");
        instance.spawnedQueueDictionary[obj.name].Enqueue(obj);
    }

    public static int GetSpawnedObjectNumber(string tag)
    {
        int count = 0;
        foreach (var item in instance.spawnedObjects)
        {
            if(item.name == tag &&
                item.activeSelf)
                count++;
        }
        return count;
        
    }


    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================

    private void Start()
    {
        StartCoroutine(E_WaitForRegisterPoolElementsFinish());
    }

    IEnumerator E_WaitForRegisterPoolElementsFinish()
    {
        yield return new WaitForEndOfFrame();
        foreach (PoolElement poolElement in poolElements)
        {
            spawnedQueueDictionary.Add(poolElement.tag, new Queue<GameObject>());
            for (int i = 0; i < poolElement.size; i++)
            {
                var obj = CreateNewObject(poolElement.tag, poolElement.prefab);
                ArrangePool(obj);
            }

            if (spawnedQueueDictionary[poolElement.tag].Count <= 0)
                Debug.Log($"Check Does {poolElement.tag} calls ReturnToPool on OnDisable()");
            else if (spawnedQueueDictionary[poolElement.tag].Count != poolElement.size)
                Debug.Log($"Check {poolElement.tag} calls ReturnToPull exactly once");

        }
    }
    GameObject Spawn(string tag, Vector2 position)
    {
        if (!spawnedQueueDictionary.ContainsKey(tag))
            throw new Exception($"Pool doesn't contains {tag}");

        Queue<GameObject> queue = spawnedQueueDictionary[tag];
        if(queue.Count <= 0)
        {
            PoolElement poolElement = poolElements.Find(x => x.tag == tag);
            var obj = CreateNewObject(poolElement.tag, poolElement.prefab);
            ArrangePool(obj);
        }
        GameObject objectToSpawn = queue.Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.identity;
        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }
    GameObject CreateNewObject(string tag, GameObject prefab)
    {
        var obj  = Instantiate(prefab, transform);
        obj.name = tag;
        obj.SetActive(false);
        return obj;
    }

    void ArrangePool(GameObject obj)
    {
        bool isSameObjExist = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == transform.childCount - 1)
            {
                obj.transform.SetSiblingIndex(i);
                spawnedObjects.Insert(i, obj);
                break;
            }
            else if (transform.GetChild(i).name == obj.name)
                isSameObjExist = true;
            else if (isSameObjExist)
            {
                obj.transform.SetSiblingIndex(i);
                spawnedObjects.Insert(i, obj);
                break;
            }
        }
    }
}

[Serializable]
public struct PoolElement
{
    public string tag;
    public GameObject prefab;
    public int size;
}