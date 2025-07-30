using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public Transform cameraTransform;

    public float backgroundHeight = 10f;
    public int prewarmCount = 2;

    private float lastY;
    private List<GameObject> backgrounds = new List<GameObject>();

    void Start()
    {
        lastY = cameraTransform.position.y - backgroundHeight;

        // 초기 배경 미리 생성
        for (int i = 0; i < prewarmCount; i++)
        {
            SpawnNextBackground();
        }
    }

    void Update()
    {
        // 카메라가 다음 배경 생성 지점에 가까워지면 새 배경 추가
        if (cameraTransform.position.y + (backgroundHeight * 1.5f) > lastY)
        {
            SpawnNextBackground();
        }
    }

    void SpawnNextBackground()
    {
        lastY += backgroundHeight;
        Vector3 spawnPos = new Vector3(0f, lastY, 0f);
        GameObject newBg = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity);
        backgrounds.Add(newBg);
    }
}