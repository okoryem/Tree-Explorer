using UnityEngine;

public class JewelTrigger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    void Start()
    {
        // Ensure the SpriteRenderer is properly assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // This method is called when another collider enters the trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that collided is on layer 6 (the character's layer)
        if (collision.gameObject.layer == 6) // Assuming layer 6 is the character
        {
            Debug.Log("Player collected the jewel!");
            Destroy(gameObject); // Completely remove the jewel from the scene
        }
    }
}
