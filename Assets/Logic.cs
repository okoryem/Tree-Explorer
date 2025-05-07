using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject startScreen;
    public GameObject explanationScreen;
    public Text jewel1Text;
    public int jewel1Count;
    public Text jewel2Text;
    public int jewel2Count;
    public Text jewel3Text;
    public int jewel3Count;

    public void startGame() {
        startScreen.SetActive(false);
        explanationScreen.SetActive(true);
    }

    public void closeExplanation() {
        explanationScreen.SetActive(false);
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
}
