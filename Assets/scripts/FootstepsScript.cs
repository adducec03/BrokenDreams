using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsScript : MonoBehaviour
{
    public AudioSource footstepsSound;

    void Update()
    {
        bool isMoving = 
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || 
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
            Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || 
            Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        footstepsSound.enabled = isMoving;
    }
}
