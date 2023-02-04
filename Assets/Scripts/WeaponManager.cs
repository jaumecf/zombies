using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    // Variable per emmagatzemar el GO de la càmera i el rang de l'arma
    public GameObject playerCam;
    public float range = 100f;

    // Mal que fa l'arma
    public float damage = 25f;

    // Per a l'animació de l'arma
    public Animator playerAnimator;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    private void Shoot()
    {
        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            //Debug.Log("Tocat!");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if(enemyManager != null)
            {
                enemyManager.Hit(damage);
            }
        }

    }
}
