using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject explanationScreen;
    public GameObject inventoryScreen;
    public GameObject tutorialGameObject; // Reference to the Tutorial GameObject
    public Text jewel1Text;
    public int jewel1Count;
    public Text jewel2Text;
    public int jewel2Count;
    public Text jewel3Text;
    public int jewel3Count;
    public GameObject miniMap; // Reference to the minimap canvas
    public GameObject miniMapPanel; // Reference to the panel inside the minimap canvas
    private bool isMiniMapActive = true;
    public TreeLogic treeLogic; // Reference to the TreeLogic script
    public Tutorial tutorial; // Reference to the Tutorial script
    public GameObject selectionScreen; // Reference to the SelectionScreen GameObject
    public GetAlgo getAlgo; // Reference to the GetAlgo script
    public GetDepth getDepth; // Reference to the GetDepth script

    private int currentDepth = 3; // Default depth for the tutorial

    private void Start()
    {
        // Ensure the Tutorial GameObject is hidden at the start
        if (tutorialGameObject != null)
        {
            tutorialGameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Tutorial GameObject is not assigned!");
        }
    }

    public void startGame()
    {
        startScreen.SetActive(false);
        explanationScreen.SetActive(true);

        // Initialize the tree and map for the tutorial using BFS as the default algorithm
        if (treeLogic != null)
        {
            treeLogic.InitializeTree(currentDepth, "BFS");
        }
        else
        {
            Debug.LogError("TreeLogic is not assigned!");
        }
    }

    public void closeExplanation()
    {
        toggleMiniMap(); // Toggle the minimap when the inventory is opened
        explanationScreen.SetActive(false);
        inventoryScreen.SetActive(true);

        // Enable the Tutorial GameObject
        if (tutorialGameObject != null)
        {
            tutorialGameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Tutorial GameObject is not assigned!");
        }

        // Start the controls tutorial
        if (tutorial != null)
        {
            tutorial.showControls();
        }
        else
        {
            Debug.LogError("Tutorial script is not assigned!");
        }
    }


    [ContextMenu("Increase Jewel 1")]
    public void addJewel1(int amount)
    {
        jewel1Count += amount;
        jewel1Text.text = jewel1Count.ToString();
    }

    [ContextMenu("Increase Jewel 2")]
    public void addJewel2(int amount)
    {
        jewel2Count += amount;
        jewel2Text.text = jewel2Count.ToString();
    }

    [ContextMenu("Increase Jewel 3")]
    public void addJewel3(int amount)
    {
        jewel3Count += amount;
        jewel3Text.text = jewel3Count.ToString();
    }

    public void toggleMiniMap()
    {
        isMiniMapActive = !isMiniMapActive;

        // Toggle the minimap's active state
        miniMap.SetActive(isMiniMapActive);

        // Reset the panel's position when the minimap is toggled on
        if (isMiniMapActive && miniMapPanel != null)
        {
            RectTransform panelRect = miniMapPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchoredPosition = new Vector2(0, -2200); // Reset the panel's position to a lower y value
            }
        }
    }

    public void StartTreeFromSelection()
    {
        if (treeLogic == null)
        {
            Debug.LogError("TreeLogic script is not assigned!");
            return;
        }

        if (getAlgo == null)
        {
            Debug.LogError("GetAlgo script is not assigned!");
            return;
        }

        if (getDepth == null)
        {
            Debug.LogError("GetDepth script is not assigned!");
            return;
        }

        // Get the selected algorithm and depth
        string selectedAlgorithm = treeLogic.selectedAlgorithm; // Algorithm set by GetAlgo
        int selectedDepth = treeLogic.depth; // Depth set by GetDepth

        // Initialize the tree with the selected algorithm and depth
        treeLogic.InitializeTree(selectedDepth, selectedAlgorithm);

        Debug.Log($"Tree initialized with algorithm: {selectedAlgorithm} and depth: {selectedDepth}");

        // Close the selection screen
        if (selectionScreen != null)
        {
            selectionScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("SelectionScreen GameObject is not assigned!");
        }
    }
}
