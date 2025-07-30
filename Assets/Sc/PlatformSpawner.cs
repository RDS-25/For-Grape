using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawner : MonoBehaviour
{
    public float breakableStartScore = 100f; // 100점부터 부서지는 발판 등장

    public GameObject platformPrefab;
    public GameObject breakablePlatformPrefab;
    public float breakableChance = 0.3f; // 부서지는 발판 확률
    public GameObject movingPlatformPrefab;
    public float movingChance = 0.3f; // 움직이는 발판 확률

    public int poolSize = 15;
    public float xRange = 2.5f;
    public float ySpacing = 1.8f;
    public Transform player;

    
    private Queue<GameObject> platformPool = new Queue<GameObject>();
    private float highestY;
    
    
    //아이템
    public GameObject[] itemPrefabs;
    public float itemSpawnChance = 0.2f; // 20%

    void Start()
    {
        highestY = player.position.y;

       
        // 화면 비율 기반으로 xRange 자동 계산
        float screenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float platformHalfWidth = platformPrefab.GetComponent<SpriteRenderer>().bounds.extents.x;
        xRange = screenHalfWidth - platformHalfWidth;

        for (int i = 0; i < poolSize; i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-xRange, xRange), i * ySpacing);
            GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
            platformPool.Enqueue(platform);
            highestY = spawnPos.y;
        }

    }

    void Update()
    {
        if (player.position.y + 10f > highestY)
        {
            RecyclePlatform();
        }
    }

    void RecyclePlatform()
    {
        GameObject oldPlatform = platformPool.Dequeue();
        Vector2 newPos = new Vector2(Random.Range(-xRange, xRange), highestY + ySpacing);

        GameObject prefabToUse = platformPrefab;
        float score = GameManager.Instance.HighestScore;

        // 구간별 난이도 설정
        int level = 0;

        if (score >= breakableStartScore * 2)      // 200점 이상: breakable + moving
            level = 2;
        else if (score >= breakableStartScore)     // 100점 이상: breakable only
            level = 1;
        else                                       // 0 ~ 99점: 기본 발판 only
            level = 0;

        float rand = Random.value;

        switch (level)
        {
            case 2:
                if (rand < breakableChance)
                    prefabToUse = breakablePlatformPrefab;
                else if (rand < breakableChance + movingChance)
                    prefabToUse = movingPlatformPrefab;
                break;

            case 1:
                if (rand < breakableChance)
                    prefabToUse = breakablePlatformPrefab;
                break;

            case 0:
            default:
                prefabToUse = platformPrefab;
                break;
        }

        Destroy(oldPlatform);
        GameObject newPlatform = Instantiate(prefabToUse, newPos, Quaternion.identity);
        platformPool.Enqueue(newPlatform);
        highestY = newPos.y;

        // 아이템은 movingPlatform 에서는 생성 안함
        if (prefabToUse != movingPlatformPrefab && Random.value < itemSpawnChance)
        {
            int randIndex = Random.Range(0, itemPrefabs.Length);
            GameObject item = Instantiate(itemPrefabs[randIndex]);
            item.transform.position = newPlatform.transform.position + Vector3.up * 0.5f;
        }
    }

}
