using UnityEngine;

public class CaveLogic : MonoBehaviour
{
    public Collider2D leftCollider;   // Collider2D for the left navigation
    public Collider2D rightCollider;  // Collider2D for the right navigation
    public Collider2D parentCollider; // Collider2D for the parent navigation

    private TreeLogic treeLogic; // Reference to TreeLogic
    private Tutorial tutorial; // Reference to the Tutorial script

    void Start()
    {
        // Find the TreeLogic component using the tag
        GameObject treeLogicObject = GameObject.FindGameObjectWithTag("TreeLogic");
        if (treeLogicObject != null)
        {
            treeLogic = treeLogicObject.GetComponent<TreeLogic>();
        }

        if (treeLogic == null)
        {
            Debug.LogError("TreeLogic component not found! Ensure the Tree Logic GameObject is tagged with 'TreeLogic' and has the TreeLogic script attached.");
        }

        // Find the Tutorial component using the tag
        GameObject tutorialObject = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorialObject != null)
        {
            tutorial = tutorialObject.GetComponent<Tutorial>();
        }

        if (tutorial == null)
        {
            Debug.LogError("Tutorial component not found! Ensure the Tutorial GameObject is tagged with 'Tutorial' and has the Tutorial script attached.");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) // Assuming layer 6 is the miner
        {
            Vector2 contactPoint = collision.transform.position;

            if (IsPointNearCollider(leftCollider, contactPoint))
            {
                Debug.Log("Left collider triggered!");
                collision.transform.position = new Vector3(0.45f, 2, 3); // Move the miner to a specific point
                treeLogic.Navigate(gameObject, "left");

                // Trigger the minimap tutorial
                if (tutorial != null)
                {
                    tutorial.showMiniMap();
                }
            }
            else if (IsPointNearCollider(rightCollider, contactPoint))
            {
                Debug.Log("Right collider triggered!");
                collision.transform.position = new Vector3(0.45f, 2, 3); // Move the miner to a specific point
                treeLogic.Navigate(gameObject, "right");
                tutorial.showExplore(); // Show the explore tutorial
            }
            else if (IsPointNearCollider(parentCollider, contactPoint))
            {
                Debug.Log("Parent collider triggered!");
                collision.transform.position = new Vector3(0, -3, 3); // Move the miner to a specific point
                treeLogic.Navigate(gameObject, "parent");
            }
            else
            {
                Debug.LogWarning("Triggered, but no collider bounds matched.");
            }
        }
    }

    bool IsPointNearCollider(Collider2D collider, Vector2 point)
    {
        Vector2 closestPoint = collider.bounds.ClosestPoint(point);
        float distance = Vector2.Distance(closestPoint, point);
        return distance < 0.5f; // Adjust threshold as needed
    }
}