using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemiesAlive == 0)
        {
            // Ronda nova
            round++;
            NextWave(round);
        }
    }

    public void NextWave(int round)
    {
        for (int i = 0; i < round; i++)
        {
            int randPos = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randPos];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }
}
