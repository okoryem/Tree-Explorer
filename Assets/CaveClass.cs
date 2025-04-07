using UnityEngine;

public class CaveClass : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject cave;
    
    public CaveClass(GameObject cave) {
        this.cave = cave;
    }
}
