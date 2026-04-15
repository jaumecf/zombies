using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{
    // Variable per al jugador FPS
    public GameObject player;

    // Variable per a l'animator del Zombie
    public Animator enemyAnimator;

    // Variable per emmagatzemar el mal que fa el Zombie
    public float damage = 20f;

    // Salut de l'enemic
    public float health = 100f;

    // Variable per a poder-nos comunicar amb el GM
    public GameManager gameManager;

    // Slider per controlar la vida del Zombie
    public Slider healthBar;

    // Animacio i millora del xoc
    public bool playerInReach;
    public float attackDelayTimer;
    public float howMuchEarlierStartAttackAnimation;
    public float delayBetweenAttacks;

    // So
    public AudioSource enemyAudioSource;
    public AudioClip[] growlAudioClips;

    // Array de jugador, ja que amb el multiplayer sinò, només persegueixen al master
    private GameObject[] playersInScene;

    public PhotonView photonView;
    void Start()
    {
        // Aquest cop, no arrossegarem la variable GameObject del FPS
        // des de l'inspector, sinò que l'assginarem des del codi
        // En concret volem cercar al jugador principal!!
        healthBar.maxValue = health;
        healthBar.value = health;
        enemyAudioSource = GetComponent<AudioSource>();
        // Iniciem la cerca optimitzada només si som el Master Client
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(UpdatePlayersListRoutine());
        }
    }

    
    void Update()
    {

        if (!enemyAudioSource.isPlaying)
        {
            enemyAudioSource.clip = growlAudioClips[Random.Range(0, growlAudioClips.Length)];
            enemyAudioSource.Play();
        }

        // Tota la lògica següent no l'executarem si no som el master client
        if(PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if(player != null)
        {
            // Accedim al component NavMeshComponent, el qual té un element que es destination de tipus Vector3
            // Li podem assignar la posició del jugador, que el tenim a la variable player gràcies al seu tranform
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = player.transform.position;

            // Animació basada en la velocitat real de l'agent
            enemyAnimator.SetBool("isRunning", agent.velocity.magnitude > 0.1f);
            
            // D'aquesta forma ens asseguram que malgrat el Zombie estigues de costat, veurem de front la barra de vida
            healthBar.transform.LookAt(player.transform);
        }

    }

    // Detectar la col·lisió
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player)
        {
            //Debug.Log("L'enemic m'ataca!!");
            //player.GetComponent<PlayerManager>().hit(damage);
            playerInReach = true;

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
            if(attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnimation && attackDelayTimer <= delayBetweenAttacks)
            {
                enemyAnimator.SetTrigger("isAttacking");
            }
            if(attackDelayTimer >= delayBetweenAttacks)
            {
                player.GetComponent<PlayerManager>().hit(damage);
                attackDelayTimer = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }

    public void Hit(float damage)
    {
        // Cridada a RPC per a sincronitzar entre els diferents clients
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            TakeDamage(damage, photonView.ViewID);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int viewID)
    {
        if(photonView.ViewID == viewID)
        {
            health -= damage;
            healthBar.value = health;
            if (health <= 0)
            {
                // Animarem el zombie
                enemyAnimator.SetTrigger("isDead");

                // Destrium a l'enemic quan la seva salut arriba a zero
                // feim referència a ell amb la variable gameObject, que fa referència al GO
                // que conté el componentn EnemyManager
                //Destroy(gameObject);
                Destroy(gameObject, 10f);
                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GetComponent<EnemyManager>());
                Destroy(GetComponent<CapsuleCollider>());

                if(!PhotonNetwork.InRoom || (PhotonNetwork.IsMasterClient && photonView.IsMine))
                {
                    //TODO : decrementar comptador enemiesAlive
                    gameManager.enemiesAlive--;
                }
            }
        }
    }
    
    IEnumerator UpdatePlayersListRoutine()
    {
        while (true)
        {
            // 1. Busquem tots els PhotonViews de l'escena
            PhotonView[] allViews = GameObject.FindObjectsOfType<PhotonView>();
            List<GameObject> tempPlayerList = new List<GameObject>();

            foreach (PhotonView view in allViews)
            {
                // 2. Si el PhotonView té el tag Player i és un personatge controlat per algú
                if (view.gameObject.CompareTag("Player"))
                {
                    tempPlayerList.Add(view.gameObject);
                }
            }

            // 3. Busquem el més proper d'aquesta llista
            float minDistance = Mathf.Infinity;
            GameObject targetTemp = null;

            foreach (GameObject p in tempPlayerList)
            {
                if (p != null)
                {
                    float distance = Vector3.Distance(p.transform.position, transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetTemp = p;
                    }
                }
            }

            player = targetTemp;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
