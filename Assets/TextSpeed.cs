using UnityEngine;
using UnityEngine.UI;

public class TextSpeed : MonoBehaviour
{
    public Text paragraph;
    public GameObject startScreen;
    public float timer = 0f;
    public float interval = 0.05f;
    private int charIndex = 0;
    private string fullText = "Welcome to Cave Explorer. We will start you off with a tutorial so you get familiar with the mechanics.";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paragraph.text = ""; // Initialize the paragraph text to empty
    }

    // Update is called once per frame
    void Update()
    {
        if (!startScreen.activeInHierarchy && charIndex < fullText.Length)
        {
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                paragraph.text += fullText[charIndex].ToString();
                charIndex++;
                timer = 0;
            }
        }
    }

    // Function to skip the typing effect and display the full text
    public void SkipTyping()
    {
        charIndex = fullText.Length; // Set charIndex to the end of the text
        paragraph.text = fullText; // Display the full text immediately
    }
}
