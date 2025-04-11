using UnityEngine;

public class CaveController : MonoBehaviour
{
    public GameObject parentCave; // Reference to the parent cave prefab
    public GameObject leftCave;   // Reference to the left child cave prefab
    public GameObject rightCave;  // Reference to the right child cave prefab
    public GameObject jewel;      // Reference to the jewel in this cave

    // Activate this cave and its jewel
    public void ActivateCave()
    {
        gameObject.SetActive(true);
        if (jewel != null)
        {
            jewel.SetActive(true);
        }
    }

    // Deactivate this cave and its jewel
    public void DeactivateCave()
    {
        if (jewel != null)
        {
            jewel.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
