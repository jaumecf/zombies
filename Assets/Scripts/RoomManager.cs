using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager sharedInstance;

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
            DontDestroyOnLoad(sharedInstance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Ens hem de subscriure a l'esdeveniment
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDisable()
    {
        // Ens hem de anul·lar la subscricpció a l'esdeveniment
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Ens hem de anul·lar la subscricpció a l'esdeveniment
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Vector3 spawnPos = new Vector3(Random.Range(-3f, 3f), 2, Random.Range(-3f, 3f));

        // En quin mode stam? Online o single player
        if (PhotonNetwork.InRoom)
        {
            //Estam online
            PhotonNetwork.Instantiate("First_Person_Player", spawnPos, Quaternion.identity);
        }
        else
        {
            Instantiate(Resources.Load("First_Person_Player"), spawnPos, Quaternion.identity);
        }
    }
}
