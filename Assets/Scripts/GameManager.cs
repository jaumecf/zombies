using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    // Referència al textMeshPro de rondes
    public TextMeshProUGUI roundText;

    // Referència al textMeshPro de rondes completades
    public TextMeshProUGUI roundsSurvivedText;

    // Referència al Panell
    public GameObject gameOverPanel;
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
            roundText.text = $"Ronda: {round}";
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

    public void GameOver()
    {
        // Activar el panell
        gameOverPanel.SetActive(true);

        // Aturar el temps
        Time.timeScale = 0;

        // Cursor visible
        Cursor.lockState = CursorLockMode.None;

        // Mostrar nº rondes
        roundsSurvivedText.text = round.ToString();
    }

    public void RestartGame()
    {
        // Recordau que carregam l'escena amb index 0, que es troba a File > Build Settings
        SceneManager.LoadScene(0);
        //Retornar l'escala de temps al valor original
        Time.timeScale = 1;
    }
}
