using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    Transform tr;
    public Vector2 spawnRangeCenter;
    public Vector2 spawnRangeSize;
    public float numMax;
    public float numSpawnSpeedLimit;
    public float spawnPeriod;
    private float spawnTimeElapsed;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private void Awake()
    {
        tr = GetComponent<Transform>();
    }
    private void Update()
    {
        int num = spawnedEnemies.Count;
        if (num < numMax)
        {
            if(num < numSpawnSpeedLimit)
            {
                if (spawnTimeElapsed > spawnPeriod)
                {
                    GameObject enemy = InstantiateOnRandomPosition(enemyPrefab);
                    spawnedEnemies.Add(enemy);
                    spawnTimeElapsed = 0;
                }
            }
            else
            {
                if (spawnTimeElapsed > spawnPeriod*(1.0f + (float)num / numSpawnSpeedLimit))
                {
                    GameObject enemy = InstantiateOnRandomPosition(enemyPrefab);
                    spawnedEnemies.Add(enemy);
                    spawnTimeElapsed = 0;
                }
            }
            spawnTimeElapsed += Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        // garbage collecting
        foreach (GameObject g in spawnedEnemies.ToArray())
        {
            if (g == null)
                spawnedEnemies.Remove(g);
        }
    }
    private GameObject InstantiateOnRandomPosition(GameObject prefab)
    {
        float xRandom = Random.Range(transform.position.x + spawnRangeCenter.x - spawnRangeSize.x/2,
                                     transform.position.x + spawnRangeCenter.x + spawnRangeSize.x/2);
        float y = transform.position.y + spawnRangeCenter.y;
        float z = transform.position.z;
        GameObject go = Instantiate(prefab, new Vector3(xRandom, y, z), Quaternion.identity);
        return go;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(spawnRangeCenter.x, spawnRangeCenter.y, 0), spawnRangeSize);
    }
}
