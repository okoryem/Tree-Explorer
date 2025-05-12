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

    // Update is called once per frame
    void Update()
    {
        // Prevent movement if the tutorial is active or the player cannot move
        if (!startScreen.activeInHierarchy && 
            !explanationScreen.activeInHierarchy && 
            !miniMapScreen.activeInHierarchy)
        {
            // Get the input from the user
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

            transform.position = transform.position + movement * Time.deltaTime * speed;
        }
    }
}
