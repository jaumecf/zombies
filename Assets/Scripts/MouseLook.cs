using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLook : MonoBehaviour
{
    public float mouseSensibility = 100f;

    // Referència al transform del jugador FPS
    public Transform playerBody;

    private float xRotation;

    // Referència al PhotonView del FPS (quan està online)
    public PhotonView photonView;

    void Start()
    {
        // Limitar a que no sortiguem dels límits de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        // Comprovam si estam online i si la referència de photonView, es d'un altre jugador...
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensibility * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensibility * Time.deltaTime;

        // diferència entre xRotation i mouseY
        xRotation -= mouseY;
        // filtram per a que no passi del màxim i del mínim
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotació eix horitzontal
        playerBody.Rotate(Vector3.up * mouseX);
        // Rotació eix vertial
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
