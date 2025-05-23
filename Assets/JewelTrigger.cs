using UnityEngine;

public class JewelTrigger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Logic logic; // Reference to the Logic script
    private Tutorial tutorial; // Reference to the Tutorial script
    public AudioSource DingSFX;

    void Start()
    {
        // Ensure the SpriteRenderer is properly assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Find the Logic script in the scene
        logic = Object.FindFirstObjectByType<Logic>();
        if (logic == null)
        {
            Debug.LogError("Logic script not found in the scene!");
        }

        // Find the Tutorial script in the scene
        tutorial = Object.FindFirstObjectByType<Tutorial>();
        if (tutorial == null)
        {
            Debug.LogError("Tutorial script not found in the scene!");
        }
    }

    // This method is called when another collider enters the trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that collided is on layer 6 (the character's layer)
        if (collision.gameObject.layer == 6) // Assuming layer 6 is the character
        {
            AudioSource.PlayClipAtPoint(DingSFX.clip, transform.position, 1.0f);
            Debug.Log($"Player collected a jewel with tag: {gameObject.tag}");

            // Call the appropriate method in the Logic script based on the tag
            if (logic != null)
            {
                if (gameObject.tag == "Jewel1")
                {
                    logic.addJewel1(1); // Add 1 to Jewel 1 count
                }
                else if (gameObject.tag == "Jewel2")
                {
                    logic.addJewel2(1); // Add 1 to Jewel 2 count
                }
                else if (gameObject.tag == "Jewel3")
                {
                    logic.addJewel3(1); // Add 1 to Jewel 3 count
                }
                else
                {
                    Debug.LogWarning("Unknown jewel tag!");
                }
            }

            // Trigger the algorithm tutorial
            if (tutorial != null)
            {
                tutorial.showAlgorithms();
            }

            // Destroy the jewel GameObject
            Destroy(gameObject);
        }
    }
}
