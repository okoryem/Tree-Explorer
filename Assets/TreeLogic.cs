using UnityEngine;

public class TreeLogic : MonoBehaviour
{
    public GameObject cavePrefab; // Reference to the cave prefab

    void Start()
    {
        TreeStructure tree = new TreeStructure();

        // Spawn a new cave prefab at position (0, 0, 0) with no rotation
        //SpawnCave(new Vector3(0, 0, 0));
    }

    /*
    GameObject SpawnCave(Vector3 position)
    {
        if (cavePrefab != null)
        {
            //Instantiate(cavePrefab, position);
        }
        else
        {
            Debug.LogError("Cave prefab is not assigned!");
        }
    }
    */

    void Update()
    {
        // Example: Spawn a cave when the player presses the spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SpawnCave(new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
        }
    }
}
