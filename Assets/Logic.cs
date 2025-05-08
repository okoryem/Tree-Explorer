using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject startScreen;
    public GameObject explanationScreen;
    public GameObject inventoryScreen;
    public Text jewel1Text;
    public int jewel1Count;
    public Text jewel2Text;
    public int jewel2Count;
    public Text jewel3Text;
    public int jewel3Count;
    public GameObject miniMap; // Reference to the minimap canvas
    public GameObject miniMapPanel; // Reference to the panel inside the minimap canvas
    private bool isMiniMapActive = false;

    public void startGame() {
        startScreen.SetActive(false);
        explanationScreen.SetActive(true);
    }

    public void closeExplanation() {
        explanationScreen.SetActive(false);
        inventoryScreen.SetActive(true);
    }

    [ContextMenu("Increase Jewel 1")]
    public void addJewel1(int amount) {
        jewel1Count += amount;
        jewel1Text.text = jewel1Count.ToString();

    }

    
    [ContextMenu("Increase Jewel 2")]
    public void addJewel2(int amount) {
        jewel2Count += amount;
        jewel2Text.text = jewel2Count.ToString();

    }

    
    [ContextMenu("Increase Jewel 3")]
    public void addJewel3(int amount) {
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
}
