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
    public GameObject titleScreen; // Prefab for the title screen
    public GameObject goodJobPopup; // Reference to the "Good Job" popup
    public GameObject tryAgainPopup; // Reference to the "Try Again" popup
    public TreeStructure tree; // Change from private to public
    private bool isNavigating = false; // Lock flag to prevent re-entry
    private LinkedList<TreeStructure.Node> correctPath = new LinkedList<TreeStructure.Node>(); // Correct path as a linked list
    private LinkedListNode<TreeStructure.Node> currentPathNode; // Tracks the current node in the linked list
    public HashSet<TreeStructure.Node> visitedNodes = new HashSet<TreeStructure.Node>(); // Tracks visited nodes
    public LinkedList<TreeStructure.Node> currentPath = new LinkedList<TreeStructure.Node>(); // Current path as a linked list

    private int depth; // Depth of the tree

    // Public method to initialize the tree logic
    public void InitializeTree(int treeDepth, string algorithm = "BFS")
    {
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
            currentPathNode = null;

            Debug.Log("Tree and associated references cleared.");
        }

        // Delete all node buttons on the minimap
        GameObject mapCanvas = GameObject.FindGameObjectWithTag("MapCanvas");
        if (mapCanvas != null)
        {
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
        }

        Debug.Log("Minimap buttons cleared.");

        // Initialize the new tree
        depth = treeDepth; // Set the depth of the tree
        tree = new TreeStructure();

        // Generate a binary tree of the specified depth
        GenerateTree(tree, depth);

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

        // Temporarily enable the MapCanvas to generate the map
        if (mapCanvas != null)
        {
            bool wasActive = mapCanvas.activeSelf;

            if (!wasActive)
            {
                mapCanvas.SetActive(true); // Temporarily enable the MapCanvas
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

            if (!wasActive)
            {
                mapCanvas.SetActive(false); // Restore the original state of MapCanvas
            }
        }
        else
        {
            Debug.LogError("MapCanvas GameObject not found! Ensure it is tagged with 'MapCanvas'.");
        }

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
        if (Input.GetKeyDown(KeyCode.V)) // Press 'V' to print visited nodes
        {
            PrintVisitedNodes();
        }
    }

    // Getter for the depth
    public int GetDepth()
    {
        return depth;
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