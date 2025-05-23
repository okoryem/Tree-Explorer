using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MapGenerator : MonoBehaviour
{
    public GameObject buttonPrefab; // Reference to the NodeButton prefab
    public Transform mapPanel; // Reference to the MapPanel
    public TreeLogic treeLogic; // Reference to the TreeLogic script
    public Logic logic;

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
            lineContainer = lineContainerObject.transform;
        }
        else
        {
            // Clear all lines in the LineContainer
            foreach (Transform line in lineContainer)
            {
                Destroy(line.gameObject);
            }
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

        buttonObject.name = node.cavePrefab.name; // Set the name of the button to the node's name

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
            OnNodeButtonClick(node.cavePrefab);

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

        // Check if the user has enough jewels to change the current cavePrefab
        if (treeLogic == null || logic == null)
        {
            Debug.LogError("TreeLogic or Logic script is not assigned!");
            return;
        }


        if (logic.jewel1Count >= 3)
        {
            logic.addJewel1(-3); // Subtract 3 jewel1s
        }
        else if (logic.jewel3Count >= 2)
        {
            logic.addJewel3(-2); // Subtract 2 jewel3s
        }
        else if (logic.jewel2Count >= 1)
        {
            logic.addJewel2(-1); // Subtract 1 jewel2
        }
        else
        {
            Debug.Log("Not enough jewels to change the current cavePrefab!");
            return; // Exit if the user doesn't have enough jewels
        }

        // Find the corresponding TreeStructure.Node for the clicked button
        TreeLogic.TreeStructure.Node targetTreeNode = treeLogic.FindNodeByGameObject(targetNode);
        if (targetTreeNode == null)
        {
            Debug.LogError("Target node not found in the tree!");
            return;
        }

        // Update node visibility
        treeLogic.UpdateNodeVisibility(targetTreeNode);

        // Update the current path in TreeLogic
        treeLogic.UpdateCurrentPath(targetTreeNode);

        // Check if the clicked node is the correct next node
        if (treeLogic.IsCorrectNode(targetTreeNode))
        {
            // Mark the node as visited
            treeLogic.visitedNodes.Add(targetTreeNode);

            Debug.Log($"Node {targetNode.name} is correct and has been added to visitedNodes.");
        }
        else
        {
            Debug.Log($"Node {targetNode.name} is incorrect.");
        }
    }

    private GameObject FindButtonForNode(TreeLogic.TreeStructure.Node node)
    {
        foreach (Transform child in mapPanel)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                Text buttonText = button.GetComponentInChildren<Text>();
                if (buttonText != null && buttonText.text == node.cavePrefab.name)
                {
                    return child.gameObject;
                }
            }
        }

        Debug.LogWarning($"Button for node '{node.cavePrefab.name}' not found!");
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate through all buttons in the map panel
        foreach (Transform child in mapPanel)
        {
            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                // Get the node name from the button's GameObject name
                string nodeName = child.name;

                // Check if the node exists in the nodeMap
                if (treeLogic.nodeMap.TryGetValue(nodeName, out TreeLogic.TreeStructure.Node node))
                {
                    // Check if the node has been visited
                    if (treeLogic.visitedNodes.Contains(node))
                    {
                        // Node has been visited: Enable the button and reset its color
                        button.interactable = true;
                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.white; // Default color
                        button.colors = colors;
                    }
                    else
                    {
                        // Node has not been visited: Disable the button and grey it out
                        button.interactable = false;
                        ColorBlock colors = button.colors;
                        colors.normalColor = Color.grey; // Greyed-out color
                        button.colors = colors;
                    }

                    // Check if this node's cavePrefab is the active one
                    if (node.cavePrefab != null && node.cavePrefab.activeSelf)
                    {
                        // Remove existing "You are here" text from all buttons
                        foreach (Transform sibling in mapPanel)
                        {
                            Text existingText = sibling.GetComponentInChildren<Text>();
                            if (existingText != null && existingText.text == "You are here")
                            {
                                Destroy(existingText.gameObject);
                            }
                        }

                        // Add "You are here" text to the current button
                        GameObject youAreHereTextObject = new GameObject("YouAreHereText");
                        youAreHereTextObject.transform.SetParent(child, false);

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
                        textRect.anchoredPosition = new Vector2(0, 25); // Position below the button
                    }
                }
                else
                {
                    Debug.LogWarning($"Node '{nodeName}' not found in nodeMap.");
                }
            }
        }
    }
}
