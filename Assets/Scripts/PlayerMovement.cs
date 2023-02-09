using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variable de Character Controller per al moviment horitzontal i vertical
    public CharacterController controller;
    //public float speed = 12f;
    public float speed;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    // Definició de variables per a la gravetat
    private Vector3 velocity;
    public float gravity = -9.81f;

    // GroundCheck
    public bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.4f; //Umbral de distància enterra
    public LayerMask groundMask;

    // Jump
    public float jumpHeight = 2f;

    void Start()
    {

    }

    void Update()
    {
        // Mirar si estic tocant el terra
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        // Valors entre -1 i 1
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Gravetat
        // Formula de velocitat = acceleració * temps^2
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        //print(velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // Veure si està esprinant o caminant
        if (Input.GetButton("Fire3") && isGrounded)
        {
            speed = runSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }
}
