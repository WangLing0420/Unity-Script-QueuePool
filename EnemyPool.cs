using System;
using System.Collections;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private Enemy enemyPrefab;
    private int initialPoolSize = 100;
    private ObjectPool<Enemy> enemyObjectPool;

    private int spawnNum = 7; // 每次生成敌人的个数
    private float spawnInterval = 0.4f; // 生成间隔时间
    private float spawnDistance = 1.5f; // 每个敌人之间的距离

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        enemyObjectPool = ObjectPool<Enemy>.CreatePool(enemyPrefab, initialPoolSize);
    }

    private void Start()
    {
        StartCoroutine(SpanwEnemy());
    }

    private IEnumerator SpanwBatch()
    {
        enemyObjectPool.BatchCreateItems();
        yield return null;
    }

    private IEnumerator SpanwEnemy()
    {
        while (true)
        {
            for (int i = 0; i < spawnNum; i++)
            {
                Enemy enemy = GetEnemy();
                if (enemy != null)
                {
                    enemy.transform.position = transform.position + new Vector3(i * spawnDistance, 0, 0); // 设置敌人位置
                    enemy.gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private Enemy GetEnemy()
    {
        if (enemyObjectPool.InactiveCount <= spawnNum)
        {
            StartCoroutine(SpanwBatch());
        }
        return enemyObjectPool.GetObject();
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemyObjectPool.ReturnObject(enemy);
        enemy.transform.position = Vector3.zero;
    }
}