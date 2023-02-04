using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    // Variable per a la salut del FPS
    public float health = 100f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Declaram un mètode per a decrementar la vida del FPs
    public void hit(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
