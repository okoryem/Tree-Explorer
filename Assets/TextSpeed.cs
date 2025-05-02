using UnityEngine;
using UnityEngine.UI;

public class TextSpeed : MonoBehaviour
{
    public Text paragraph;
    public GameObject startScreen;
    public float timer = 0f;
    public float interval = 0.05f;
    private int charIndex = 0;
    private string fullText = "Welcome to Cave Explorer. Your goal is to explore this cave using a Breadth First Search (BFS). Start by moving to the bottom left corner of the cave. Open Chest (PRESS E) to collect jewels. But you can only open chest if you are in a cave that follows the BFS. If you are in a cave that does not follow the BFS, you will be stuck in the cave forever...Just kidding. You can go back to previous nodes and go to the correct path. Remember BFS is a tree traversal algorithm that explores all the nodes at the present depth prior to moving on to the nodes at the next depth level. You can use this algorithm to explore the cave. Good luck!";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paragraph.text = ""; // Initialize the paragraph text to empty
    }

    // Update is called once per frame
    void Update()
    {
        if (!startScreen.activeInHierarchy && charIndex < fullText.Length)
        {
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                paragraph.text += fullText[charIndex].ToString();
                charIndex++;
                timer = 0;
            }
        }
    }

    // Function to skip the typing effect and display the full text
    public void SkipTyping()
    {
        charIndex = fullText.Length; // Set charIndex to the end of the text
        paragraph.text = fullText; // Display the full text immediately
    }
}
