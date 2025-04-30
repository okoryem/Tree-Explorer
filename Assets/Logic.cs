using UnityEngine;

public class Logic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject startScreen;
    public GameObject explanationScreen;
    public void startGame() {
        startScreen.SetActive(false);
        explanationScreen.SetActive(true);
    }

    public void closeExplanation() {
        explanationScreen.SetActive(false);
    }
}
