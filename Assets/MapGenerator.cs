using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public GameObject buttonPrefab; // Reference to the NodeButton prefab
    public Transform mapPanel; // Reference to the MapPanel
    public TreeLogic treeLogic; // Reference to the TreeLogic script

    void Start()
    {
        if (treeLogic == null)
        {
            Debug.LogError("TreeLogic reference is missing!");
            return;
        }

        GenerateMap();
    }

    public void GenerateMap()
    {
        // Clear any existing buttons in the map panel
        foreach (Transform child in mapPanel)
        {
            Destroy(child.gameObject);
        }

        // Start generating the map from the root node
        TreeLogic.TreeStructure.Node rootNode = treeLogic.tree.GetRoot(); // Fully qualify TreeStructure
        if (rootNode == null)
        {
            Debug.LogError("Tree root is null!");
            return;
        }

        // Get the depth of the tree using the GetDepth method
        int maxDepth = treeLogic.GetDepth();

        // Calculate the total width and height of the tree
        float totalWidth = Mathf.Pow(2, maxDepth - 1) * 160 + (Mathf.Pow(2, maxDepth - 1) - 1) * 20; // Width of bottom layer
        float totalHeight = maxDepth * 140 + (maxDepth - 1) * 50; // Total height of the tree

        // Calculate the starting position for the root node
        RectTransform panelRect = mapPanel.GetComponent<RectTransform>();
        float startX = 0; // Centered horizontally
        float startY = panelRect.rect.height / 2 - totalHeight / 2; // Start near the top of the panel

        // Recursively generate buttons for the tree
        GenerateNodeButton(rootNode, startX, startY, totalWidth, 0, maxDepth); // Start at (x=startX, y=startY)
    }

    private void GenerateNodeButton(TreeLogic.TreeStructure.Node node, float x, float y, float levelWidth, int depth, int maxDepth)
    {
        if (node == null) return;

        // Create a button for the current node
        GameObject buttonObject = Instantiate(buttonPrefab, mapPanel);
        Button button = buttonObject.GetComponent<Button>();

        // Set the button's text to the node's name
        Text buttonText = buttonObject.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = node.cavePrefab.name;
        }

        // Position the button on the map
        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(x, y);

        // Calculate vertical spacing dynamically
        float verticalSpacing = 140 + 50; // Height of button + 20 units of vertical space

        // If this is not the bottom-most layer, calculate child positions
        if (depth < maxDepth - 1)
        {
            // Calculate the horizontal spacing for the current level
            float childLevelWidth = levelWidth / 2;

            // Recursively generate buttons for the left and right children
            if (node.left != null)
            {
                GenerateNodeButton(node.left, x - childLevelWidth / 2, y - verticalSpacing, childLevelWidth, depth + 1, maxDepth);
            }
            if (node.right != null)
            {
                GenerateNodeButton(node.right, x + childLevelWidth / 2, y - verticalSpacing, childLevelWidth, depth + 1, maxDepth);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
