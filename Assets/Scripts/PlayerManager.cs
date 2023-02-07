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

    // Variable per a poder controlar la càmera
    public GameObject playerCamera;

    // Variable per controlar el temps de vibració de la càmera
    private float shakeTime = 1f;
    private float shakeDuration = 0.5f;
    private Quaternion playerCameraOriginalRotation;

    // Referència al component CanvasGroup del Hit Panel
    public CanvasGroup hitPanel;
    void Start()
    {
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
    }

    void Update()
    {
        if(hitPanel.alpha > 0)
        {
            hitPanel.alpha -= Time.deltaTime;
        }
        if(shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }else if(playerCamera.transform.localRotation != playerCameraOriginalRotation)
        {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }
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
        else
        {
            shakeTime = 0;
            hitPanel.alpha = 1;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0, 0);
    }
}
