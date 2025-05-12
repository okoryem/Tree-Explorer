using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Animator animator;
    public float speed = 2;
    public GameObject startScreen;
    public GameObject explanationScreen;
    public GameObject miniMapScreen;
    public Tutorial tutorial; // Reference to the Tutorial script
    public AudioSource footstepsSound;

    // Update is called once per frame
    void Update()
    {
        // Prevent movement if the tutorial is active or the player cannot move
        if (!startScreen.activeInHierarchy && 
            !explanationScreen.activeInHierarchy && 
            !miniMapScreen.activeInHierarchy)
        {
            // Get the input from the user
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveX, moveY, 0.0f);

            animator.SetFloat("Horizontal", moveX);
            animator.SetFloat("Vertical", moveY);

            transform.position += movement * Time.deltaTime * speed;

            // Play footstep sound if there is movement
            if (movement.magnitude > 0.01f)
            {
                if (!footstepsSound.enabled)
                    footstepsSound.enabled = true;
            }
            else
            {
                if (footstepsSound.enabled)
                    footstepsSound.enabled = false;
            }
        }
    }
}
