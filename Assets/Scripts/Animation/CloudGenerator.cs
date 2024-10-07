using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] clouds;
    [SerializeField] float spawnInterval;
    [SerializeField] GameObject endPoint;
    [SerializeField] GameObject startPos;

    void Start()
    {
        Prewarm();
        Invoke("AttemptSpawn", spawnInterval);
    }

    void SpawnCloud(Vector3 startPos)
    {
        int randomIndex = UnityEngine.Random.Range(0, clouds.Length);
        GameObject cloud = Instantiate(clouds[randomIndex]);
        cloud.transform.SetParent(GameObject.Find("Clouds").transform);
	cloud.AddComponent<Cloud>();

        float startY = UnityEngine.Random.Range(startPos.y - 15f, startPos.y + 15f);
        cloud.transform.position = new Vector3(startPos.x, startY, startPos.z);

        float scale = UnityEngine.Random.Range(0.5f, 1.5f);
        cloud.transform.localScale = new Vector2(scale, scale);

        float speed = UnityEngine.Random.Range(10.5f, 25.5f);
        cloud.GetComponent<Cloud>().StartFloating(speed, endPoint.transform.position.x);
    }

    void AttemptSpawn()
    {
        //check some things.
        SpawnCloud(startPos.transform.position);
        Invoke("AttemptSpawn", spawnInterval);
    }

    void Prewarm()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 spawnPos = startPos.transform.position + Vector3.right * (i * 2);
            SpawnCloud(spawnPos);
        }
    }

}
