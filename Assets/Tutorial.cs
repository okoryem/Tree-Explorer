using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public bool isTutorialActive = false;
    public Text tutorialText;
    public GameObject selectionScreen; // Reference to the SelectionScreen GameObject
    public GameObject miniMapPanel; // Reference to the minimap panel

    private float timer = 0f;
    private float interval = 0.05f; // Time between each character
    private int charIndex = 0;
    private string currentMessage = ""; // The current message being typed
    private bool isTyping = false;

    // Flags to ensure tutorials are called only once
    private bool hasShownInventory = false;
    private bool hasShownAlgorithms = false;
    private bool hasShownMiniMap = false; // New flag for minimap tutorial
    private bool movementDetected = false; // Tracks if movement keys have been pressed
    private float movementTimer = 0f; // Tracks time since movement started
    private bool isMovementTimerRunning = false; // Tracks if the movement timer is active
    private bool hasShownExplore = false; // New flag for explore tutorial
    //private bool hasShownDFS = false; // Flag to ensure showDFS runs only once


    // Function to show controls tutorial
    public void showControls()
    {
        isTutorialActive = true;
        StartTyping("Use the arrow keys or WASD to move around the cave.");
    }

    // Function to show collection tutorial
    public void showCollection()
    {
        isTutorialActive = true;
        StartTyping("Nice you got movement down! Now head over to the chest and press E. Collect the gems.");
    }

    // Function to show inventory tutorial
    public void showInventory()
    {
        if (hasShownInventory) return; // Prevent this from running multiple times

        isTutorialActive = true;
        hasShownInventory = true; // Mark the inventory tutorial as shown
        StartTyping("Your goal is to leave the cave with as many gems as possible.");
        showAlgorithms();
    }

    // Function to show minimap tutorial
    public void showMiniMap()
    {
        if (hasShownMiniMap) return; // Prevent this from running multiple times

        isTutorialActive = true;
        hasShownMiniMap = true; // Mark the minimap tutorial as shown
        StartTyping("Use the minimap to navigate through the cave system. Now go back to the root and head to the right.");
    }

    // Function to show explore tutorial
    public void showExplore()
    {
        if (hasShownExplore || !hasShownMiniMap) return; // Prevent this from running multiple times and ensure showMiniMap was called

        isTutorialActive = true;
        hasShownExplore = true; // Mark the explore tutorial as shown
        StartTyping("Great! Now let's explore the cave further using a BFS algorithm.");
    }

    // Function to show algorithms tutorial
    public void showAlgorithms()
    {
        if (hasShownAlgorithms) return; // Prevent this from running multiple times

        isTutorialActive = true;
        hasShownAlgorithms = true; // Mark the algorithm tutorial as shown
        StartTyping("Ok let's start exploring the cave in a Breadth First algorithm. The goal of BFS is to explore each depth fully before moving on to the next. Now, head to the bottom left corner.");

        // Generate a new tree with BFS
        TreeLogic treeLogic = Object.FindFirstObjectByType<TreeLogic>();
        if (treeLogic != null)
        {
            treeLogic.InitializeTree(3, "BFS"); // Generate a new tree with BFS
        }
        else
        {
            Debug.LogError("TreeLogic component not found! Ensure it is properly assigned in the scene.");
        }
    }

    public void showDFS()
    {
        Debug.Log("DFS tutorial triggered!"); // Debug log to confirm the function is called
        if (TreeLogic.hasShownDFS1) return; // Prevent this from running multiple times

        isTutorialActive = true;
        TreeLogic.hasShownDFS1 = true; // Mark the DFS tutorial as shown globally

        // Start typing the DFS tutorial message
        StartTyping("Depth First Search (DFS) explores as far as possible along each branch before backtracking. Let's explore the cave using DFS!");

        // Generate a new tree with DFS
        TreeLogic treeLogic = UnityEngine.Object.FindFirstObjectByType<TreeLogic>();
        if (treeLogic != null)
        {
            miniMapPanel.SetActive(true); // Show the minimap panel
            treeLogic.InitializeTree(3, "DFS"); // Generate a new tree with DFS
            miniMapPanel.SetActive(false); // Hide the minimap panel
        }
        else
        {
            Debug.LogError("TreeLogic component not found! Ensure it is properly assigned in the scene.");
        }
    }

    // Function to end the tutorial
    public void endTutorial()
    {
        isTutorialActive = false;
        tutorialText.text = ""; // Clear the tutorial text
    }

    // Function to start the typing effect
    private void StartTyping(string message)
    {
        currentMessage = message;
        charIndex = 0;
        timer = 0f;
        isTyping = true;
        tutorialText.text = ""; // Clear the text before starting
    }

    // Update is called once per frame
    void Update()
    {
        // Handle typing effect
        if (isTyping && charIndex < currentMessage.Length)
        {
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                tutorialText.text += currentMessage[charIndex].ToString();
                charIndex++;
                timer = 0;
            }
        }
        else if (isTyping && charIndex >= currentMessage.Length)
        {
            isTyping = false; // Typing is complete
        }

        // Detect movement keys and start the movement timer
        if (!movementDetected && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                                  Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                                  Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                                  Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            movementDetected = true;
            isMovementTimerRunning = true;
            movementTimer = 0f; // Reset the movement timer
        }

        // Run the movement timer
        if (isMovementTimerRunning)
        {
            movementTimer += Time.deltaTime;

            if (movementTimer >= 5f) // After 5 seconds of movement
            {
                isMovementTimerRunning = false; // Stop the timer
                showCollection(); // Show the collection tutorial
            }
        }

        
    }

}
