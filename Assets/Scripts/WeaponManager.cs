using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class WeaponManager : MonoBehaviour, IOnEventCallback
{

    // Variable per emmagatzemar el GO de la càmera i el rang de l'arma
    public GameObject playerCam;
    public float range = 100f;

    // Mal que fa l'arma
    public float damage = 25f;

    // Per a l'animació de l'arma
    public Animator playerAnimator;

    // Referència per a gestionar el sistema de particules
    public ParticleSystem flashParticleSystem;
    public GameObject bloodParticleSystem;

    //Efectes de so
    public AudioClip shootClip;
    public AudioSource weaponAudioSource;

    // Referència al PhotonView del FPS (quan està online)
    public PhotonView photonView;

    // Referència al GameManager pel multijugador
    public GameManager gameManager;

    // Codi per al Raise Event
    private const byte VFX_EVENT = 0;

    void Start()
    {
        weaponAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        // Comprovam si estam online i si la referència de photonView, es d'un altre jugador...
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        if (!gameManager.isPaused && !gameManager.isGameOver)
        {
            if (playerAnimator.GetBool("isShooting"))
            {
                playerAnimator.SetBool("isShooting", false);
            }
            if (Input.GetButtonDown("Fire1"))
            {
                //Debug.Log("Pium!");
                Shoot();
            }
        }
    }

    public void ShootVFX(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            flashParticleSystem.Play();
            weaponAudioSource.PlayOneShot(shootClip, 1f);
        }
    }

    private void Shoot()
    {
        if (PhotonNetwork.InRoom)
        {
            // Els RPC només cerquen a la mateixa altura del pare, i no als fills
            // el sistema de partícules es fill de l'arma i no ho trobaria...
            int viewID = photonView.ViewID;

            RaiseEventOptions raiseOptions = new RaiseEventOptions{ Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            PhotonNetwork.RaiseEvent(VFX_EVENT, viewID, raiseOptions, sendOptions);
            //Ara ja no necessitam cridar un RPC per a que els demés jugador vegin els VFX
            //photonView.RPC("WeaponShootSFX", RpcTarget.All, photonView.ViewID);
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }

        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            //Debug.Log("Tocat!");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if(enemyManager != null)
            {
                enemyManager.Hit(damage);
                // generam una instància del particle system, en el punt on hem ferit al Zombie,
                // i fent que l'animació sempre estigui rotada en direcció al tret
                GameObject particleInstance = Instantiate(bloodParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
                // Feim que la instància sigui filla del Zombie al qual hem ferit
                particleInstance.transform.parent = hit.transform;
                // Recordau que aquesta animació te seleccionat per Stop Action: "Destroy" ja que sinó es crearien infinites instàncies
            }
        }
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == VFX_EVENT)
        {
            Debug.Log("EventReceived");
            int viewID = (int)photonEvent.CustomData;
            ShootVFX(viewID);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
