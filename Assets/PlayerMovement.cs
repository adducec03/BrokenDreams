using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 inputFromKeyboard;
    private Animator animator;
    public Joystick joystick; // Riferimento al joystick virtuale
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        animator=GetComponentInChildren<Animator>();
    }

    // Update is called once per frameq
    void Update()
{
    // Se l'input da tastiera è attivo, usa quello, altrimenti joystick
    Vector2 finalInput = inputFromKeyboard.magnitude > 0.1f 
        ? inputFromKeyboard 
        : new Vector2(joystick.Horizontal, joystick.Vertical);

    rb.linearVelocity = finalInput * moveSpeed;

    if (finalInput.magnitude > 0.1f)
    {
        animator.SetBool("isWalking", true);
        animator.SetFloat("InputX", finalInput.x);
        animator.SetFloat("InputY", finalInput.y);
        animator.SetFloat("LastInputX", finalInput.x);
        animator.SetFloat("LastInputY", finalInput.y);
    }
    else
    {
        animator.SetBool("isWalking", false);
    }
}

    // Callback della Input System (via InputActions)
    public void OnMove(InputAction.CallbackContext context)
    {
    inputFromKeyboard = context.ReadValue<Vector2>();
    }

    /*public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

    }*/
}
