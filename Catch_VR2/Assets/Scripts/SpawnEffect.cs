using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{


    public GameObject portals;
    public Vector3[] spawnPosition = new Vector3[3];

    public int minMob;
    public int maxMob;

    public int minSpawn;
    public int maxSpawn;

    public float timer;
    public float timerToInstantiate;
    public float timerMaxStart;
    public float timerMin;
    public float timerMax;

    public bool isSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        timerToInstantiate = Random.Range(0.5f, timerMaxStart);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerToInstantiate && isSpawning == false)
        {
            isSpawning = true;
            Debug.Log("lama");
            int randomNumberPos = Random.Range(0, spawnPosition.Length);
            for (int i = 0; i <= randomNumberPos; i++)
            {
                int randomMobGenerator = Random.Range(minMob, maxMob);
                for (int j = 0; j <= randomMobGenerator; j++)
                {
                    int randomPos = Random.Range(0, spawnPosition.Length);
                    float intX = Random.Range(spawnPosition[randomPos].x + minSpawn, spawnPosition[randomPos].x +maxSpawn);
                    float intY = Random.Range(spawnPosition[randomPos].z + minSpawn, spawnPosition[randomPos].z + maxSpawn);
                    Vector3 randomToAdd = new Vector3(intX,0, intY);
                    Debug.LogWarning("x"+intX);
                    Debug.LogWarning("y"+intY);

                    Instantiate(portals, randomToAdd, Quaternion.identity);
                    Debug.Log("JE TE SPAWN");                    
                }
                if (i >= randomNumberPos)
                {
                    isSpawning = false;
                    timer = 0;
                    timerToInstantiate = Random.Range(timerMin, timerMax);
                }
            }
        }
    }
}
