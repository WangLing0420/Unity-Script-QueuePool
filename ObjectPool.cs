using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private static ObjectPool<T> instance;

    private T objectPrefab; // 对象预制体
    private Queue<T> objectPool = new Queue<T>(); // 对象池
    private int defaultSize = 50; // 初始池默认大小
    private int inactiveCount = 0; // 当前未激活对象的数量
    private const int batchSize = 10; // 每次扩容个数
    private const int SHRINK_THRESHOLD = 20; // 当空闲对象超过此值时触发收缩

    // 状态监控属性
    public int InactiveCount => inactiveCount;
    
    // 私有构造函数，防止外部直接实例化
    private ObjectPool(T prefab, int size)
    {
        objectPrefab = prefab;
        defaultSize = size;
        InitializePool();
    }

    // 创建和初始化对象池
    public static ObjectPool<T> CreatePool(T prefab, int size = 50)
    {
        if (instance == null)
        {
            instance = new ObjectPool<T>(prefab, size);
        }
        return instance;
    }

    // 初始化对象池
    private void InitializePool()
    {
        for (int i = 0; i < defaultSize; i++)
        {
            CreateNewObject();
        }
    }

    // 执行收缩操作
    public IEnumerator ShrinkPool()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);

            if (inactiveCount > SHRINK_THRESHOLD && objectPool.Count > defaultSize)
            {
                int itemsToDestroy = inactiveCount - defaultSize;
                for (int i = 0; i < itemsToDestroy && objectPool.Count > defaultSize; i++)
                {
                    T obj = objectPool.Dequeue();
                    Object.Destroy(obj.gameObject);
                    inactiveCount--;
                }
            }
        }
    }

    // 批量创建对象
    public void BatchCreateItems()
    {
        for (int i = 0; i < batchSize; i++)
        {
            CreateNewObject();
        }
    }

    // 创建单个对象
    private T CreateNewObject()
    {
        T obj = Object.Instantiate(objectPrefab);
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj);
        inactiveCount++;
        return obj;
    }

    // 从对象池中获取对象
    public T GetObject()
    {
        if (objectPool.Count > 0)
        {
            T obj = objectPool.Dequeue();
            obj.gameObject.SetActive(true);
            inactiveCount--;
            return obj;
        }

        return CreateNewObject();
    }

    // 将对象返回到对象池
    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        objectPool.Enqueue(obj);
        inactiveCount++;
    }
}