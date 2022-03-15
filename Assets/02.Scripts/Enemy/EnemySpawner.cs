using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] PoolElement poolElment;
    Transform tr;
    public Vector2 spawnRangeCenter;
    public Vector2 spawnRangeSize;
    public int numMax;
    public int numSpawnSpeedLimit;
    public float spawnPeriod;
    private float spawnTimeElapsed;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        if (poolElment.size > numMax)
            poolElment.size = numMax;

        ObjectPool.instance.AddPoolElement(poolElment);
    }
    private void Update()
    {
        int num = ObjectPool.GetSpawnedObjectNumber(poolElment.tag);
        if (num < numMax)
        {
            if(num < numSpawnSpeedLimit)
            {
                if (spawnTimeElapsed > spawnPeriod)
                {
                    SpawnOnRandomPosition(poolElment.tag);
                    spawnTimeElapsed = 0;
                }
            }
            else
            {
                if (spawnTimeElapsed > spawnPeriod*(1.0f + (float)num / numSpawnSpeedLimit))
                {
                    SpawnOnRandomPosition(poolElment.tag);
                    spawnTimeElapsed = 0;
                }
            }
            spawnTimeElapsed += Time.deltaTime;
        }
    }
    private void SpawnOnRandomPosition(string tag)
    {
        float xRandom = Random.Range(tr.position.x + spawnRangeCenter.x - spawnRangeSize.x/2,
                                     tr.position.x + spawnRangeCenter.x + spawnRangeSize.x/2);
        float y = tr.position.y + spawnRangeCenter.y;
        float z = tr.position.z;
        ObjectPool.SpawnFromPool(tag, new Vector3(xRandom, y, z));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + new Vector3(spawnRangeCenter.x, spawnRangeCenter.y, 0), spawnRangeSize);
    }
}
