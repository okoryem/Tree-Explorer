using UnityEngine;
using System.Collections.Generic;

public class TreeLogic : MonoBehaviour
{
    public GameObject caveDepth1; // Prefab for depth 1
    public GameObject caveDepth2; // Prefab for depth 2
    public GameObject caveDepth3; // Prefab for depth 3
    private TreeStructure tree;

    void Start()
    {
        tree = new TreeStructure();

        // Generate a binary tree of depth 3
        GenerateTree(tree, 3);

        // Spawn the tilemaps based on the tree structure
        SpawnTilemaps(tree);
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
        tree.Insert(new CaveClass(rootCave));

        // Recursively generate the rest of the tree
        GenerateTreeRecursive(tree.GetRoot(), depth - 1);
    }

    void GenerateTreeRecursive(TreeStructure.Node node, int depth)
    {
        if (depth <= 0) return;

        // Select the appropriate prefab based on the depth
        GameObject prefabToUse = GetPrefabForDepth(depth);

        // Create left child
        GameObject leftCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity);
        leftCave.name = node.cave.cave.name + "_Left";
        var leftCaveClass = leftCave.GetComponent<CaveClass>();
        node.left = new TreeStructure.Node(new CaveClass(leftCave), node);
        leftCaveClass.node = node.left; // Link the node to the CaveClass

        // Create right child
        GameObject rightCave = Instantiate(prefabToUse, Vector3.zero, Quaternion.identity);
        rightCave.name = node.cave.cave.name + "_Right";
        var rightCaveClass = rightCave.GetComponent<CaveClass>();
        node.right = new TreeStructure.Node(new CaveClass(rightCave), node);
        rightCaveClass.node = node.right; // Link the node to the CaveClass

        // Recurse for left and right children
        GenerateTreeRecursive(node.left, depth - 1);
        GenerateTreeRecursive(node.right, depth - 1);
    }

    GameObject GetPrefabForDepth(int depth)
    {
        // Return the appropriate prefab based on the depth
        switch (depth)
        {
            case 3: return caveDepth3;
            case 2: return caveDepth2;
            case 1: return caveDepth1;
            default: return caveDepth1; // Fallback to depth 1 prefab
        }
    }

    void SpawnTilemaps(TreeStructure tree)
    {
        if (tree == null || tree.GetRoot() == null) return;

        // Use BFS to spawn tilemaps at appropriate positions
        Queue<TreeStructure.Node> queue = new Queue<TreeStructure.Node>();
        queue.Enqueue(tree.GetRoot());

        Vector3 startPosition = Vector3.zero;
        float xOffset = 5f; // Horizontal spacing between nodes
        float yOffset = -5f; // Vertical spacing between levels

        while (queue.Count > 0)
        {
            TreeStructure.Node current = queue.Dequeue();

            // Calculate position based on depth and order
            int depth = GetDepth(current);
            Vector3 position = startPosition + new Vector3(depth * xOffset, depth * yOffset, 0);

            // Move the tilemap to the calculated position
            current.cave.cave.transform.position = position;

            // Enqueue children
            if (current.left != null) queue.Enqueue(current.left);
            if (current.right != null) queue.Enqueue(current.right);
        }
    }

    int GetDepth(TreeStructure.Node node)
    {
        int depth = 0;
        while (node.parent != null)
        {
            depth++;
            node = node.parent;
        }
        return depth;
    }
}
