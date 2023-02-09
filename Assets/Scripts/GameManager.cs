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

    // Referència al Panell de Game Over
    public GameObject gameOverPanel;
    private int roundsSurvided;

    // Referència al Panell de Pausa
    public GameObject pausePanel;

    // Referència al Panell de Fade In Out
    public Animator fadePanelAnimator;

    // Referència per activar-lo i desactivar-lo pels menus
    public WeaponManager weaponManager;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
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
        roundsSurvided = round - 1;
        roundsSurvivedText.text = roundsSurvided.ToString();

        weaponManager.enabled = false;
    }

    public void RestartGame()
    {
        // Recordau que carregam l'escena amb index 0, que es troba a File > Build Settings
        // Després el modificam per 1
        SceneManager.LoadScene(1);
        //Retornar l'escala de temps al valor original
        Time.timeScale = 1;
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1;
        AudioListener.volume = 1;
        fadePanelAnimator.SetTrigger("fadeIn");
        // Per a que doni temps a veure l'animació, canviarem d'escena amb una mica de delay
        Invoke("LoadMainMenuScene", 0.5f);
        //SceneManager.LoadScene(0);
    }

    public void LoadMainMenuScene()
    {

        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        weaponManager.enabled = false;
        if (gameOverPanel.activeSelf != true)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        AudioListener.volume = 0;

    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        weaponManager.enabled = true;
    }
}
