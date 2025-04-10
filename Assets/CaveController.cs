using UnityEngine;

public class CaveController : MonoBehaviour
{
    public GameObject parentCave; // Reference to the parent cave prefab
    public GameObject leftCave;   // Reference to the left child cave prefab
    public GameObject rightCave;  // Reference to the right child cave prefab

    // Optional: Add methods to activate/deactivate this cave
    public void ActivateCave()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateCave()
    {
        gameObject.SetActive(false);
    }
}
