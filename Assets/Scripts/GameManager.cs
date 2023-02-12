using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive;
    public int round;

    public GameObject[] spawnPoints;
    //public GameObject enemyPrefab;

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

    // referències per siubstituir la refèrencia al weapon manager
    public bool isPaused;
    public bool isGameOver;

    public PhotonView photonView;
    /*
     * Deshabilitam el Singleton, ja que en multijugador, no ens interessa,
     * la idea es que hi hagi una sòla instància de game instance per sala
     * i no per jugador... per tant el "host" o bàsicament el primer que 
     * crei la sala serà qui ho inicialitzarà
    public static GameManager sharedInstance;

    private void Awake()
    {
        if(sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
    void Start()
    {
        isPaused = false;
        isGameOver = false;
        Time.timeScale = 1;

        // Inicialitzam els swapn points
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
    }

    // Update is called once per frame
    void Update()
    {
        // Farem la instanciació de Game Manager si no estam online, o be hi estam i nosaltres som el host
        if (!PhotonNetwork.InRoom || (PhotonNetwork.IsMasterClient && photonView.IsMine))
        {
            if (enemiesAlive == 0)
            {
                // Ronda nova
                round++;
                NextWave(round);
                if (PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("CurrentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRround(round);
                }
                
            }
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

            GameObject enemyInstance;
            if (PhotonNetwork.InRoom)
            {
                enemyInstance = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemyInstance = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            
            enemyInstance.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;
        }
    }

    public void GameOver()
    {
        // Activar el panell
        gameOverPanel.SetActive(true);

        if (!PhotonNetwork.InRoom)
        {
            // Aturar el temps si no estam online
            Time.timeScale = 0;
        }
        
        // Cursor visible
        Cursor.lockState = CursorLockMode.None;

        // Mostrar nº rondes
        roundsSurvided = round - 1;
        roundsSurvivedText.text = roundsSurvided.ToString();
        isGameOver = true;

        
    }

    public void RestartGame()
    {
        // Recordau que carregam l'escena amb index 0, que es troba a File > Build Settings
        // Després el modificam per 1
        SceneManager.LoadScene(1);
        //Retornar l'escala de temps al valor original
        if (!PhotonNetwork.InRoom)
        {
            // Aturar el temps si no estam online
            Time.timeScale = 1;
        }
    }

    public void BackMainMenu()
    {
        /* bug
        if (!PhotonNetwork.InRoom)
        {
            // Aturar el temps si no estam online
            Time.timeScale = 1;
        }*/
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
        isPaused = true;
        
        if (gameOverPanel.activeSelf != true)
        {
            pausePanel.SetActive(true);
            if (!PhotonNetwork.InRoom)
            {
                // Aturar el temps si no estam online
                Time.timeScale = 0;
            }
            Cursor.lockState = CursorLockMode.None;
        }
        AudioListener.volume = 0;

    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        if (!PhotonNetwork.InRoom)
        {
            // Tornam a temps a la normalitat si no estam online
            Time.timeScale = 1;
        }
        Cursor.lockState = CursorLockMode.Locked;
        AudioListener.volume = 1;
        isPaused = false;
        
    }

    private void DisplayNextRround(int round)
    {
        roundText.text = $"Ronda: {round}";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (photonView.IsMine)
        {
            if(changedProps["CurrentRound"] != null)
            {
                DisplayNextRround((int) changedProps["CurrentRound"]);
            }
        }
    }
}
