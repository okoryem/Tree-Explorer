using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Animator animator;
    public float speed = 2;
    public GameObject startScreen;
    public GameObject explanationScreen;
    public 

    // Update is called once per frame
    void Update()
    {
        if(!startScreen.activeInHierarchy && !explanationScreen.activeInHierarchy) {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

            transform.position = transform.position + movement * Time.deltaTime * speed;
        }
    }
}
