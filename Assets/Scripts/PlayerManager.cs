using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    // Variable per a la salut del FPS
    public float health = 100f;

    // Variable per modificar l'etiqueta de texte de salut
    public TextMeshProUGUI healthText;

    // Variable per poder accedir al nostre GameManager
    public GameManager gameManager;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // Declaram un mètode per a decrementar la vida del FPs
    public void hit(float damage)
    {
        health -= damage;
        healthText.text = $"{health} HP";
        if(health <= 0)
        {
            //SceneManager.LoadScene(0);
            gameManager.GameOver();
        }
    }
}
