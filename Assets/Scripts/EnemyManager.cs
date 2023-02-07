using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    public bool playerInReach;
    public float attackDelayTimer;
    public float howMuchEarlierStartAttackAnimation;
    public float delayBetweenAttacks;
    void Start()
    {
        // Aquest cop, no arrossegarem la variable GameObject del FPS
        // des de l'inspector, sinò que l'assginarem des del codi
        // En concret volem cercar al jugador principal!!
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar.maxValue = health;
        healthBar.value = health;
    }

    
    void Update()
    {
        // Accedim al component NavMeshComponent, el qual té un element que es destination de tipus Vector3
        // Li podem assignar la posició del jugador, que el tenim a la variable player gràcies al seu tranform
        GetComponent<NavMeshAgent>().destination = player.transform.position;

        // D'aquesta forma ens asseguram que malgrat el Zombie estigues de costat, veurem de front la barra de vida
        healthBar.transform.LookAt(player.transform);

        // En primer lloc hem d'accedir a la velocitat del Zombiem, des del component NavMeshAgent
        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
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
        health -= damage;
        healthBar.value = health;
        if(health <= 0)
        {
            // Animarem el zombie
            enemyAnimator.SetTrigger("isDead");

            // Destrium a l'enemic quan la seva salut arriba a zero
            // feim referència a ell amb la variable gameObject, que fa referència al GO
            // que conté el componentn EnemyManager
            //Destroy(gameObject);
            Destroy(gameObject,10f);
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());


            //TODO : decrementar comptador enemiesAlive
            gameManager.enemiesAlive--;

            
        }
    }
}
