using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
        // Clear any existing buttons in the map panel, but keep the LineContainer
        foreach (Transform child in mapPanel)
        {
            if (child.name != "LineContainer")
            {
                Destroy(child.gameObject);
            }
        }

        // Ensure the LineContainer exists
        Transform lineContainer = mapPanel.Find("LineContainer");
        if (lineContainer == null)
        {
            GameObject lineContainerObject = new GameObject("LineContainer");
            lineContainerObject.transform.SetParent(mapPanel, false);
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
        rectTransform.anchoredPosition = new Vector2(x, y - 500); // Lower all buttons by 200 pixels

        // Add an Outline component for hover states
        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = Color.clear; // Default to no outline
        outline.effectDistance = new Vector2(15, 15); // Adjust thickness of the outline

        // Add hover functionality
        EventTrigger trigger = buttonObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((eventData) => { outline.effectColor = Color.yellow; }); // Hover color
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((eventData) => { outline.effectColor = Color.clear; }); // Reset color
        trigger.triggers.Add(entryExit);

        // Add a click event to the button
        button.onClick.AddListener(() =>
        {
            treeLogic.OnNodeButtonClick(node.cavePrefab);

            // Remove "You are here" text from all buttons
            foreach (Transform child in mapPanel)
            {
                Text existingText = child.GetComponentInChildren<Text>();
                if (existingText != null && existingText.text == "You are here")
                {
                    Destroy(existingText.gameObject);
                }
            }

            // Add "You are here" text below the current button
            GameObject youAreHereTextObject = new GameObject("YouAreHereText");
            youAreHereTextObject.transform.SetParent(buttonObject.transform, false);

            Text youAreHereText = youAreHereTextObject.AddComponent<Text>();
            youAreHereText.text = "You are here";

            // Set the font to Bangers
            Font bangersFont = Resources.Load<Font>("Fonts/Bangers");
            if (bangersFont != null)
            {
                youAreHereText.font = bangersFont;
            }
            else
            {
                Debug.LogWarning("Bangers font not found! Using default font.");
                youAreHereText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            // Set font size and alignment
            youAreHereText.fontSize = 22;
            youAreHereText.alignment = TextAnchor.MiddleCenter;

            // Adjust the position of the text
            RectTransform textRect = youAreHereText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0);
            textRect.anchorMax = new Vector2(0.5f, 0);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = new Vector2(0, 25); // Shift the text box down by -125
        });

        // Draw lines to child nodes
        if (depth < maxDepth - 1)
        {
            // Calculate vertical spacing dynamically
            float verticalSpacing = 140 + 20; // Height of button + 20 units of vertical space
            float childLevelWidth = levelWidth / 2;

            // Recursively generate buttons for the left and right children
            if (node.left != null)
            {
                Vector2 childPosition = new Vector2(x - childLevelWidth / 2, y - verticalSpacing - 500);
                DrawLine(rectTransform.anchoredPosition, childPosition);
                GenerateNodeButton(node.left, x - childLevelWidth / 2, y - verticalSpacing, childLevelWidth, depth + 1, maxDepth);
            }
            if (node.right != null)
            {
                Vector2 childPosition = new Vector2(x + childLevelWidth / 2, y - verticalSpacing - 500);
                DrawLine(rectTransform.anchoredPosition, childPosition);
                GenerateNodeButton(node.right, x + childLevelWidth / 2, y - verticalSpacing, childLevelWidth, depth + 1, maxDepth);
            }
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        // Find or create the LineContainer
        Transform lineContainer = mapPanel.Find("LineContainer");
        if (lineContainer == null)
        {
            GameObject containerObject = new GameObject("LineContainer");
            containerObject.transform.SetParent(mapPanel, false);
            lineContainer = containerObject.transform;
        }

        // Create a new GameObject for the line
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(lineContainer, false);

        // Add an Image component
        Image lineImage = lineObject.AddComponent<Image>();
        lineImage.color = Color.black; // Set the line color

        // Set the RectTransform properties
        RectTransform rectTransform = lineObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Calculate the position and rotation of the line
        Vector2 direction = end - start;
        float distance = direction.magnitude;
        rectTransform.sizeDelta = new Vector2(distance, 5f); // Set the width of the line (5f makes it thicker)
        rectTransform.anchoredPosition = start + direction / 2;
        rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    public void OnNodeButtonClick(GameObject targetNode)
    {
        Debug.Log($"Node clicked: {targetNode.name}");
        // Existing logic...
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
