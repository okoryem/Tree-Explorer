using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TreeLogic : MonoBehaviour
{
    public GameObject caveDepth1; // Prefab for depth 1
    public GameObject caveDepth2; // Prefab for depth 2
    public GameObject caveDepth3; // Prefab for depth 3
    public GameObject jewelPrefab; // Prefab for the jewel
    public GameObject titleScreen; // Prefav for the title screen
    public GameObject goodJobPopup; // Reference to the "Good Job" popup
    public GameObject tryAgainPopup; // Reference to the "Try Again" popup
    private TreeStructure tree;
    private bool isNavigating = false; // Lock flag to prevent re-entry
    private LinkedList<TreeStructure.Node> correctPath = new LinkedList<TreeStructure.Node>(); // Correct path as a linked list
    private LinkedListNode<TreeStructure.Node> currentPathNode; // Tracks the current node in the linked list
    private HashSet<TreeStructure.Node> visitedNodes = new HashSet<TreeStructure.Node>(); // Tracks visited nodes
    private LinkedList<TreeStructure.Node> currentPath = new LinkedList<TreeStructure.Node>(); // Current path as a linked list

    void Start()
    {
        tree = new TreeStructure();

        // Generate a binary tree of depth 3
        GenerateTree(tree, 3);

        // Hide all caves that are not part of the route
        HideNonRouteCaves(tree);

        // Generate the correct path based on BFS
        GenerateCorrectPath();

        // Traverse the tree in BFS order and log the names of the nodes
        Debug.Log("BFS Traversal of the Tree:");
        tree.TraverseBFS(node =>
        {
            if (node.cavePrefab != null)
            {
                Debug.Log(node.cavePrefab.name);
            }
        });
    }

    public void startGame(){ // Function for start button on title screen
        titleScreen.SetActive(false);
        Debug.Log("Start Button Pressed!");
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

            // Deactivate all caves
            tree.TraverseBFS(node =>
            {
                if (node.cavePrefab != null)
                {
                    node.cavePrefab.SetActive(false);
                }
            });

            // Activate the target node's cave
            targetNode.cavePrefab.SetActive(true);

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
    private bool IsCorrectNode(TreeStructure.Node targetNode)
    {
        // Check if the target node matches the next node in the linked list
        if (currentPathNode != null && currentPathNode.Value == targetNode)
        {
            visitedNodes.Add(targetNode); // Mark the node as visited
            currentPathNode = currentPathNode.Next; // Move to the next node in the linked list
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

        // Create the root node using the first prefab
        GameObject rootCave = Instantiate(caveDepth1, Vector3.zero, Quaternion.identity);
        rootCave.name = "Root";

        // Create the root node and link it to the GameObject
        TreeStructure.Node rootNode = new TreeStructure.Node(rootCave);

        // Insert the root node into the tree
        tree.SetRoot(rootNode);

        // Recursively generate the rest of the tree
        GenerateTreeRecursive(rootNode, depth - 1);
    }

    void GenerateTreeRecursive(TreeStructure.Node node, int depth)
    {
        if (depth <= 0) return;

        GameObject prefabToUse = GetPrefabForDepth(depth);

        // Create left child
        GameObject leftCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity); // Spawn at the same position
        leftCave.name = node.cavePrefab.name + "_Left";
        node.left = new TreeStructure.Node(leftCave, node);

        // Spawn a jewel in the left cave
        GameObject leftJewel = Instantiate(jewelPrefab, leftCave.transform.position, Quaternion.identity);
        leftJewel.name = leftCave.name + "_Jewel";
        leftJewel.transform.SetParent(leftCave.transform); // Parent the jewel to the cave
        leftJewel.SetActive(false); // Keep the jewel initially inactive

        // Assign relationships in the CaveController script
        var leftController = leftCave.GetComponent<CaveController>();
        if (leftController != null)
        {
            leftController.parentCave = node.cavePrefab; // Set parent
            node.cavePrefab.GetComponent<CaveController>().leftCave = leftCave; // Set left child
            leftController.jewel = leftJewel; // Assign the jewel to the CaveController
        }

        // Create right child
        GameObject rightCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity); // Spawn at the same position
        rightCave.name = node.cavePrefab.name + "_Right";
        node.right = new TreeStructure.Node(rightCave, node);

        // Spawn a jewel in the right cave
        GameObject rightJewel = Instantiate(jewelPrefab, rightCave.transform.position, Quaternion.identity);
        rightJewel.name = rightCave.name + "_Jewel";
        rightJewel.transform.SetParent(rightCave.transform); // Parent the jewel to the cave
        rightJewel.SetActive(false); // Keep the jewel initially inactive

        // Assign relationships in the CaveController script
        var rightController = rightCave.GetComponent<CaveController>();
        if (rightController != null)
        {
            rightController.parentCave = node.cavePrefab; // Set parent
            node.cavePrefab.GetComponent<CaveController>().rightCave = rightCave; // Set right child
            rightController.jewel = rightJewel; // Assign the jewel to the CaveController
        }

        // Recurse for left and right children
        GenerateTreeRecursive(node.left, depth - 1);
        GenerateTreeRecursive(node.right, depth - 1);
    }

    // Spawns a jewel in the given cave
    void SpawnJewel(GameObject cave)
    {
        if (jewelPrefab != null)
        {
            // Instantiate the jewel at the cave's position
            GameObject jewel = Instantiate(jewelPrefab, cave.transform.position, Quaternion.identity);
            jewel.name = cave.name + "_Jewel";

            // Parent the jewel to the cave for better organization
            jewel.transform.SetParent(cave.transform);

            // Keep the jewel initially inactive
            jewel.SetActive(false);

            // Assign the jewel to the CaveController for activation/deactivation
            var caveController = cave.GetComponent<CaveController>();
            if (caveController != null)
            {
                caveController.jewel = jewel;
            }
        }
        else
        {
            Debug.LogWarning("Jewel prefab is not assigned!");
        }
    }

    GameObject GetPrefabForDepth(int depth)
    {
        // Return the appropriate prefab based on the depth
        switch (depth)
        {
            case 4: 
                return caveDepth1; // Depth 4 uses caveDepth1 again
            case 3: 
                return caveDepth3; // Depth 3 uses caveDepth3
            case 2: 
                return caveDepth2; // Depth 2 uses caveDepth2
            case 1: 
                return caveDepth1; // Depth 1 uses caveDepth1
            default: 
                Debug.LogWarning($"Invalid depth {depth}. Defaulting to caveDepth1.");
                return caveDepth1; // Fallback to caveDepth1
        }
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

    private void UpdateCurrentPath(TreeStructure.Node targetNode)
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