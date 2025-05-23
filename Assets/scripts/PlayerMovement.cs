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
    public Joystick joystick;
    public bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    // Traccia del player
    public GameObject trailPrefab;
    public float trailInterval = 0.2f;
    private float nextTrailTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
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
            StartFootsteps();
        }
        else
        {
            animator.SetBool("isWalking", false);
            StopFootsteps();
        }

        // Lascia la traccia ogni tot secondi
        if (Time.time >= nextTrailTime)
        {
            LeaveTrail();
            nextTrailTime = Time.time + trailInterval;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputFromKeyboard = context.ReadValue<Vector2>();
    }

    void LeaveTrail()
    {
        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        trail.tag = "TrailPoint"; // Assegna il tag per il nemico
        Destroy(trail, 5f); // La traccia scompare dopo 5 secondi
    }

    void StartFootsteps()
    {
        if (playingFootsteps) return;
        playingFootsteps = true;
        float speedFactor = moveSpeed > 0 ? 1f / moveSpeed : 0.5f;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed * speedFactor);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep");
    }
    
    public Vector2 GetLastDirection()
    {
        if (moveInput.magnitude > 0.1f)
            return moveInput.normalized;

        return new Vector2(animator.GetFloat("LastInputX"), animator.GetFloat("LastInputY"));
    }
}