using UnityEngine;
using System.Collections;

public class PlayerCollisionHandler : MonoBehaviour
{
    public GameObject miner; // Reference to the miner GameObject
    public Vector3 teleportPosition = new Vector3(0, 0, 0); // The center position for every cave

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has a CaveClass component
        CaveClass caveClass = other.GetComponentInParent<CaveClass>();
        if (caveClass != null && caveClass.node != null)
        {
            // Determine which collider was triggered
            if (other == caveClass.parentTrigger)
            {
                Debug.Log("Parent trigger activated");
                ShowNode(caveClass.node.parent);
            }
            else if (other == caveClass.leftTrigger)
            {
                Debug.Log("Left trigger activated");
                ShowNode(caveClass.node.left);
            }
            else if (other == caveClass.rightTrigger)
            {
                Debug.Log("Right trigger activated");
                ShowNode(caveClass.node.right);
            }

            // Example: Play an animation or sound
            if (miner != null)
            {
                Animator minerAnimator = miner.GetComponent<Animator>();
                if (minerAnimator != null)
                {
                    minerAnimator.SetTrigger("EnterCave");
                }
            }
        }

        // Check if the collided object has a trigger collider
        if (other.CompareTag("CaveTrigger")) // Ensure the cave colliders are tagged as "CaveTrigger"
        {
            Debug.Log("Collided with a cave trigger!");

            // Teleport the character to the center of the next cave
            transform.position = teleportPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CaveClass caveClass = other.GetComponent<CaveClass>();
        if (caveClass != null && caveClass.node != null)
        {
            TreeStructure.Node node = caveClass.node;

            // Hide the parent, left, and right nodes
            HideNode(node.parent);
            HideNode(node.left);
            HideNode(node.right);
        }
    }

    private void ShowNode(TreeStructure.Node node)
    {
        if (node != null && node.cave != null)
        {
            // Enable the GameObject associated with the node
            node.cave.cave.SetActive(true);
        }
    }

    private void HideNode(TreeStructure.Node node)
    {
        if (node != null && node.cave != null)
        {
            node.cave.cave.SetActive(false);
        }
    }

    private IEnumerator MoveToNode(Vector3 targetPosition)
    {
        float duration = 0.5f; // Time to move
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}