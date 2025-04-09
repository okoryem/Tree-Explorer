using UnityEngine;

public class CaveClass : MonoBehaviour
{
    public GameObject cave; // Reference to the cave GameObject
    public TreeStructure.Node node; // Reference to the corresponding tree node

    // References to the colliders for parent, left, and right
    public Collider parentTrigger;
    public Collider leftTrigger;
    public Collider rightTrigger;
}
