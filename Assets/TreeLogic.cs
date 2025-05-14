using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TreeLogic : MonoBehaviour
{
    public GameObject caveDepth1; // Prefab for depth 1
    public GameObject caveDepth2; // Prefab for depth 2
    public GameObject caveDepth3; // Prefab for depth 3
    public GameObject titleScreen; // Prefab for the title screen
    public GameObject goodJobPopup; // Reference to the "Good Job" popup
    public GameObject tryAgainPopup; // Reference to the "Try Again" popup
    public GameObject mapCanvas; // Reference to the MapCanvas GameObject
    public GameObject selectionScreen; // Reference to the SelectionScreen GameObject
    public GameObject tutorialGameObject; // Reference to the Tutorial GameObject

    public TreeStructure tree; // Change from private to public
    private bool isNavigating = false; // Lock flag to prevent re-entry
    private LinkedList<TreeStructure.Node> correctPath = new LinkedList<TreeStructure.Node>(); // Correct path as a linked list
    private LinkedListNode<TreeStructure.Node> currentPathNode; // Tracks the current node in the linked list
    public HashSet<TreeStructure.Node> visitedNodes = new HashSet<TreeStructure.Node>(); // Tracks visited nodes
    public LinkedList<TreeStructure.Node> currentPath = new LinkedList<TreeStructure.Node>(); // Current path as a linked list
    public Dictionary<string, TreeStructure.Node> nodeMap = new Dictionary<string, TreeStructure.Node>(); // Dictionary to store nodes by name

    public int depth = 3; // Default depth of the tree
    public string selectedAlgorithm = "BFS"; // Default algorithm

    // Global variable to track if showDFS has been shown
    public static bool hasShownDFS1 = false;
    public Logic logic;

    public Text timerText; // Reference to the TimerText UI element

    // Timer variables
    private float timerDuration = 0f;
    private float timer = 0f;
    private bool isTimerActive = false;

    public GameObject endScreen; // Reference to the EndScreen GameObject
    public Text endScreenMessageText; // Reference to the message Text on the EndScreen
    public Text endScreenGemsText; // Reference to the gems Text on the EndScreen

    // Function to check if all nodes are visited and handle logic for showDFS and selectionScreen
    public void CheckAllNodesVisited()
    {
        Debug.Log("hasShowenDFS:" + hasShownDFS1);
        Debug.Log("AreAllCorrectNodesVisited:" + AreAllCorrectNodesVisited());
        if (AreAllCorrectNodesVisited())
        {

            if (!hasShownDFS1)
            {
                // Run showDFS if it hasn't been shown yet
                Tutorial tutorial = UnityEngine.Object.FindFirstObjectByType<Tutorial>();
                if (tutorial != null)
                {
                    tutorial.showDFS();
                }
                // Mark showDFS as shown
            }
            else
            {
                // Show the SelectionScreen for subsequent visits
                if (selectionScreen != null && !selectionScreen.activeSelf)
                {
                    selectionScreen.SetActive(true);
                    tutorialGameObject.SetActive(false); // Hide the tutorial GameObject
                    Debug.Log("SelectionScreen is now visible.");
                }
            }
        }
    }

    // Public method to initialize the tree logic
    public void InitializeTree(int treeDepth, string algorithm = "BFS")
    {        // Reset jewel counts
        if (logic != null)
        {
            logic.addJewel1(-logic.jewel1Count); // Reset jewel1 count to 0
            logic.addJewel2(-logic.jewel2Count); // Reset jewel2 count to 0
            logic.addJewel3(-logic.jewel3Count); // Reset jewel3 count to 0
        }

        if (mapCanvas == null)
        {
            Debug.LogError("MapCanvas is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Enable the MapCanvas at the start
        mapCanvas.SetActive(true);

        // Check if there is an existing tree and map
        if (tree != null && tree.GetRoot() != null)
        {
            Debug.Log("Existing tree and map found. Deleting them...");

            // Delete all cave GameObjects associated with the tree
            tree.TraverseBFS(node =>
            {
                if (node.cavePrefab != null)
                {
                    Destroy(node.cavePrefab); // Destroy the cave GameObject
                }
            });

            // Clear the tree structure
            tree = new TreeStructure();
            visitedNodes.Clear();
            currentPath.Clear();
            correctPath.Clear();
            nodeMap.Clear();
            currentPathNode = null;

            Debug.Log("Tree and associated references cleared.");
        }

        // Delete all node buttons on the minimap
        Transform mapPanel = mapCanvas.transform.Find("MapPanel");
        if (mapPanel != null)
        {
            foreach (Transform child in mapPanel)
            {
                if (child.name != "LineContainer") // Keep the LineContainer if it exists
                {
                    Destroy(child.gameObject); // Destroy the button GameObject
                }
            }
        }

        Debug.Log("Minimap buttons cleared.");

        // Initialize the new tree
        depth = treeDepth; // Set the depth of the tree
        tree = new TreeStructure();

        // Generate a binary tree of the specified depth
        GenerateTree(tree, depth);

        tree.TraverseBFS(node => { if (node.cavePrefab != null) nodeMap[node.cavePrefab.name] = node; });

        // Hide all caves that are not part of the route
        HideNonRouteCaves(tree);

        // Generate the correct path based on the chosen algorithm
        if (algorithm.ToUpper() == "DFS")
        {
            GenerateCorrectPathDFS();
        }
        else
        {
            GenerateCorrectPath(); // Default to BFS
        }

        // Generate the map
        MapGenerator mapGenerator = mapCanvas.GetComponentInChildren<MapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap(); // Call GenerateMap() here
        }
        else
        {
            Debug.LogError("MapGenerator instance not found on MapCanvas!");
        }

        // Start the timer for the tree
        StartTimer(treeDepth);

        // Traverse the tree in BFS order and log the names of the nodes
        Debug.Log("BFS Traversal of the Tree:");
        tree.TraverseBFS(node =>
        {
            if (node.cavePrefab != null)
            {
                Debug.Log(node.cavePrefab.name);
            }
        });

        // Disable the MapCanvas at the end
        mapCanvas.SetActive(false);
    }

    private void StartTimer(int treeDepth)
    {
        if (treeDepth == 3)
        {
            isTimerActive = false; // No timer for depth 3
            if (timerText != null)
            {
                timerText.gameObject.SetActive(false); // Hide the timer text
            }
            Debug.Log("Timer is disabled for depth 3.");
            return;
        }

        timerDuration = 90f + (treeDepth - 3) * 30f; // Start at 1:30 for depth 4, increase by 30 seconds per depth
        timer = timerDuration;
        isTimerActive = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true); // Show the timer text
        }

        Debug.Log($"Timer started for depth {treeDepth}: {timerDuration} seconds.");
    }

    private void HandleTimerExpiry()
    {
        if (endScreen != null)
        {
            // Show the EndScreen
            endScreen.SetActive(true);

            // Set the message text based on whether all nodes were visited
            if (AreAllCorrectNodesVisited())
            {
                endScreenMessageText.text = "You explored the cave!";
            }
            else
            {
                endScreenMessageText.text = "Timer ran out, try again.";
            }

            // Calculate the total number of gems
            int totalJewels = logic.jewel1Count + logic.jewel2Count + logic.jewel3Count;
            endScreenGemsText.text = $"You ended with {totalJewels} jewel(s)";

            // Start a coroutine to handle the delay before showing the SelectionScreen
            StartCoroutine(ShowSelectionScreenAfterEndScreen());
        }

        isTimerActive = false; // Stop the timer
    }

    private IEnumerator ShowSelectionScreenAfterEndScreen()
    {
        // Wait for 5 seconds (duration of the EndScreen)
        yield return new WaitForSeconds(5f);

        // Hide the EndScreen
        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }

        // Show the SelectionScreen
        if (selectionScreen != null)
        {
            selectionScreen.SetActive(true);
            Debug.Log("Timer expired! Showing SelectionScreen.");
        }
        else
        {
            Debug.LogError("SelectionScreen is not assigned!");
        }
    }

    // Function to search for a node by its GameObject
    public TreeStructure.Node FindNodeByGameObject(GameObject cave)
    {
        TreeStructure.Node foundNode = null;

        tree.TraverseBFS(node =>
        {
            if (node.cavePrefab == cave)
            {
                foundNode = node;
            }
        });

        return foundNode;
    }

    // Function to navigate to a specific direction (left, right, or parent)
    public void Navigate(GameObject currentCave, string direction)
    {
        if (isNavigating) return; // Prevent re-entry
        isNavigating = true;

        try
        {
            TreeStructure.Node currentNode = FindNodeByGameObject(currentCave);

            if (currentNode == null)
            {
                Debug.LogError("Node not found for the given GameObject!");
                return;
            }

            TreeStructure.Node targetNode = null;

            switch (direction.ToLower())
            {
                case "left":
                    targetNode = currentNode.left;
                    break;
                case "right":
                    targetNode = currentNode.right;
                    break;
                case "parent":
                    targetNode = currentNode.parent;
                    break;
                default:
                    Debug.LogError("Invalid direction specified!");
                    return;
            }

            if (targetNode == null)
            {
                Debug.LogError($"No {direction} node exists for the current node!");
                return;
            }

            // Update the current path and compare it with the correct path
            UpdateCurrentPath(targetNode);

            // Update node visibility
            UpdateNodeVisibility(targetNode);

            // Optionally, teleport the player to the target cave's position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = targetNode.cavePrefab.transform.position;
                Debug.Log($"Player teleported to {direction} node: {targetNode.cavePrefab.name}");
            }
        }
        finally
        {
            isNavigating = false; // Always reset the flag
        }
    }

    // Helper method to determine if the chosen node is correct
    public bool IsCorrectNode(TreeStructure.Node targetNode)
    {
        // Check if the target node matches the next node in the correct path
        if (currentPathNode != null && currentPathNode.Value == targetNode)
        {
            visitedNodes.Add(targetNode); // Mark the node as visited
            currentPathNode = currentPathNode.Next; // Move to the next node in the correct path
            return true; // Correct choice
        }

        return false; // Incorrect choice
    }

    // Helper method to show a popup
    private void ShowPopup(GameObject popup)
    {
        popup.SetActive(true);
        StartCoroutine(HidePopupAfterDelay(popup, 2f)); // Hide after 2 seconds
    }

    // Coroutine to hide a popup after a delay
    private IEnumerator HidePopupAfterDelay(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        popup.SetActive(false);
    }

    void GenerateTree(TreeStructure tree, int depth)
    {
        if (caveDepth1 == null || caveDepth2 == null || caveDepth3 == null)
        {
            Debug.LogError("One or more cave prefabs are not assigned!");
            return;
        }

        // Create the root node using caveDepth1
        GameObject rootCave = Instantiate(caveDepth1, Vector3.zero, Quaternion.identity);
        rootCave.name = "Root";

        // Create the root node and link it to the GameObject
        TreeStructure.Node rootNode = new TreeStructure.Node(rootCave, null); // Explicitly set parent to null

        // Insert the root node into the tree
        tree.SetRoot(rootNode);

        // Recursively generate the rest of the tree
        GenerateTreeRecursive(rootNode, depth - 1, depth);
    }

    void GenerateTreeRecursive(TreeStructure.Node node, int depth, int maxDepth)
    {
        if (depth <= 0) return;

        GameObject prefabToUse = GetPrefabForDepth(depth, maxDepth);

        // Create left child
        GameObject leftCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity);
        leftCave.name = node.cavePrefab.name + "_Left";
        leftCave.tag = "CaveNode"; // Assign the "CaveNode" tag
        node.left = new TreeStructure.Node(leftCave, node);

        // Create right child
        GameObject rightCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity);
        rightCave.name = node.cavePrefab.name + "_Right";
        rightCave.tag = "CaveNode"; // Assign the "CaveNode" tag
        node.right = new TreeStructure.Node(rightCave, node);

        // Recurse for left and right children
        GenerateTreeRecursive(node.left, depth - 1, maxDepth);
        GenerateTreeRecursive(node.right, depth - 1, maxDepth);
    }

    GameObject GetPrefabForDepth(int depth, int maxDepth)
    {
        // Root node (topmost level) always uses caveDepth1
        if (depth == maxDepth)
        {
            return caveDepth1;
        }

        // Alternate between caveDepth2 and caveDepth3 for other depths
        return ((maxDepth - depth) % 2 == 0) ? caveDepth2 : caveDepth3;
    }

    void HideNonRouteCaves(TreeStructure tree)
    {
        if (tree == null || tree.GetRoot() == null) return;

        // Use BFS to traverse the tree and hide all caves
        Queue<TreeStructure.Node> queue = new Queue<TreeStructure.Node>();
        queue.Enqueue(tree.GetRoot());

        while (queue.Count > 0)
        {
            TreeStructure.Node current = queue.Dequeue();

            // Hide the cave prefab
            current.cavePrefab.SetActive(false);

            // Enqueue children
            if (current.left != null) queue.Enqueue(current.left);
            if (current.right != null) queue.Enqueue(current.right);
        }

        // Enable only the root cave initially
        tree.GetRoot().cavePrefab.SetActive(true);
    }

    private void GenerateCorrectPath()
    {
        correctPath.Clear(); // Clear any existing path
        visitedNodes.Clear(); // Clear visited nodes

        if (tree == null || tree.GetRoot() == null) return;

        Queue<TreeStructure.Node> queue = new Queue<TreeStructure.Node>();
        TreeStructure.Node rootNode = tree.GetRoot();

        // Add the root node to visited nodes
        visitedNodes.Add(rootNode);

        queue.Enqueue(rootNode);

        bool isRoot = true; // Flag to skip the root node in the correct path

        while (queue.Count > 0)
        {
            TreeStructure.Node current = queue.Dequeue();

            // Skip the root node in the correct path
            if (isRoot)
            {
                isRoot = false;
            }
            else
            {
                correctPath.AddLast(current); // Add non-root nodes to the correct path
            }

            // Enqueue children
            if (current.left != null) queue.Enqueue(current.left);
            if (current.right != null) queue.Enqueue(current.right);
        }

        Debug.Log("Correct Path (BFS, excluding root): " + string.Join(", ", correctPath.Select(node => node.cavePrefab.name)));
    }

    public void GenerateCorrectPathDFS()
    {
        correctPath.Clear(); // Clear any existing path
        visitedNodes.Clear(); // Clear visited nodes

        if (tree == null || tree.GetRoot() == null) return;

        TreeStructure.Node rootNode = tree.GetRoot();

        // Add the root node to visited nodes
        visitedNodes.Add(rootNode);

        // Use a stack for DFS traversal
        Stack<TreeStructure.Node> stack = new Stack<TreeStructure.Node>();
        stack.Push(rootNode);

        bool isRoot = true; // Flag to skip the root node in the correct path

        while (stack.Count > 0)
        {
            TreeStructure.Node current = stack.Pop();

            // Skip the root node in the correct path
            if (isRoot)
            {
                isRoot = false;
            }
            else
            {
                correctPath.AddLast(current); // Add non-root nodes to the correct path
            }

            // Push right child first so that left child is processed first
            if (current.right != null) stack.Push(current.right);
            if (current.left != null) stack.Push(current.left);
        }

        Debug.Log("Correct Path (DFS, excluding root): " + string.Join(", ", correctPath.Select(node => node.cavePrefab.name)));
    }

    public void UpdateCurrentPath(TreeStructure.Node targetNode)
    {
        // Check if the target node has already been visited
        if (visitedNodes.Contains(targetNode))
        {
            Debug.Log($"Node {targetNode.cavePrefab.name} has already been visited. Skipping correctness check and no popup will be shown.");
            return; // Exit early if the node has already been visited
        }

        // Add the target node to the current path
        currentPath.AddLast(targetNode);

        // Compare the current path with the correct path
        if (currentPath.Count > correctPath.Count)
        {
            Debug.Log($"Incorrect Path: Current path exceeds the correct path. Correct node was: {correctPath.Last.Value.cavePrefab.name}");
            ShowPopup(tryAgainPopup); // Show "Try Again" popup if the current path exceeds the correct path
            currentPath.RemoveLast(); // Remove the incorrect node
            return;
        }

        var correctPathNode = correctPath.First;
        var currentPathNode = currentPath.First;

        while (currentPathNode != null && correctPathNode != null)
        {
            if (currentPathNode.Value != correctPathNode.Value)
            {
                Debug.Log($"Incorrect Path: Current path does not match the correct path. Correct node was: {correctPathNode.Value.cavePrefab.name}");
                ShowPopup(tryAgainPopup); // Show "Try Again" popup if the paths don't match
                currentPath.RemoveLast(); // Remove the incorrect node
                return;
            }

            currentPathNode = currentPathNode.Next;
            correctPathNode = correctPathNode.Next;
        }

        // If the paths match so far, show the "Good Job" popup
        Debug.Log($"Correct Path: Current path matches the correct path so far. Next correct node is: {correctPathNode?.Value.cavePrefab.name ?? "None"}");
        ShowPopup(goodJobPopup);

        // Add the target node to visited nodes only if it is correct
        visitedNodes.Add(targetNode);

        // Log the current and correct paths for debugging
        Debug.Log("Current Path: " + string.Join(", ", currentPath.Select(node => node.cavePrefab.name)));
        Debug.Log("Correct Path: " + string.Join(", ", correctPath.Select(node => node.cavePrefab.name)));
    }

    public void PrintVisitedNodes()
    {
        if (visitedNodes.Count == 0)
        {
            Debug.Log("No nodes have been visited yet.");
            return;
        }

        string visitedNodeNames = string.Join(", ", visitedNodes.Select(node => node.cavePrefab.name));
        Debug.Log($"Visited Nodes: {visitedNodeNames}");
    }

    public void OnNodeButtonClick(GameObject targetNode)
    {
        // Hide the current node
        if (currentPathNode != null && currentPathNode.Value.cavePrefab != null)
        {
            currentPathNode.Value.cavePrefab.SetActive(false);
        }

        // Show the target node
        targetNode.SetActive(true);

        // Update the current node to the target node
        TreeStructure.Node targetTreeNode = FindNodeByGameObject(targetNode);
        if (targetTreeNode != null)
        {
            currentPathNode = new LinkedListNode<TreeStructure.Node>(targetTreeNode);
        }

        Debug.Log($"Navigated to node: {targetNode.name}");
    }

    public void UpdateNodeVisibility(TreeStructure.Node activeNode)
    {
        // Traverse all nodes in the tree and deactivate them
        tree.TraverseBFS(node =>
        {
            if (node.cavePrefab != null)
            {
                node.cavePrefab.SetActive(false); // Deactivate all nodes
            }
        });

        // Activate only the active node
        if (activeNode != null && activeNode.cavePrefab != null)
        {
            activeNode.cavePrefab.SetActive(true);
            Debug.Log($"Node {activeNode.cavePrefab.name} is now active.");
        }
    }

    public bool AreAllCorrectNodesVisited()
    {
        // Check if the correct path is empty
        if (correctPath.Count == 0)
        {
            Debug.LogWarning("Correct path is empty. Cannot verify if all nodes are visited.");
            return false; // Return false if there is no correct path
        }

        // Check if all nodes in the correct path have been visited
        foreach (var node in correctPath)
        {
            if (!visitedNodes.Contains(node))
            {
                return false; // If any node in the correct path is not visited, return false
            }
        }

        // All nodes in the correct path have been visited
        Debug.Log("All nodes in the correct path have been visited in the correct order!");
        return true;
    }

    void Update()
    {
        // Update the timer
        if (isTimerActive)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                HandleTimerExpiry();
            }

            // Update the timer text
            UpdateTimerText();
        }

        // Call CheckAllNodesVisited to check if all correct nodes have been visited
        CheckAllNodesVisited();

        // Debugging: Press 'V' to print visited nodes
        if (Input.GetKeyDown(KeyCode.V))
        {
            PrintVisitedNodes();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}"; // Format as MM:SS
        }
    }

    // Getter for the depth
    public int GetDepth()
    {
        return depth;
    }

    // Method to set the algorithm
    public void SetAlgorithm(string algorithm)
    {
        selectedAlgorithm = algorithm;
    }

    // Update the InitializeTree method to use the selected algorithm
    public void InitializeTree(int treeDepth)
    {
        InitializeTree(treeDepth, selectedAlgorithm);
    }

    // Method to set the depth
    public void SetDepth(int newDepth)
    {
        depth = newDepth;
        Debug.Log($"Tree depth set to: {depth}");
    }

    // Update the InitializeTree method to use the current depth
    public void InitializeTree(string algorithm = "BFS")
    {
        InitializeTree(depth, algorithm);
    }

    // Inner TreeStructure class
    public class TreeStructure
    {
        private Node root;

        public class Node
        {
            public GameObject cavePrefab; // Reference to the cave GameObject
            public Node left;             // Left child
            public Node right;            // Right child
            public Node parent;           // Parent node

            public Node(GameObject cavePrefab, Node parent = null)
            {
                this.cavePrefab = cavePrefab;
                this.parent = parent;
                this.left = null;
                this.right = null;
            }
        }

        public void SetRoot(Node rootNode)
        {
            root = rootNode;
        }

        public Node GetRoot()
        {
            return root;
        }

        public void TraverseBFS(Action<Node> action)
        {
            if (root == null) return;

            var queue = new Queue<Node>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();
                action(current);

                if (current.left != null) queue.Enqueue(current.left);
                if (current.right != null) queue.Enqueue(current.right);
            }
        }

    }
}