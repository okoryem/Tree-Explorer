using UnityEngine;
using UnityEngine.UI;

public class TextSpeed : MonoBehaviour
{
    public Text paragraph;
    public GameObject startScreen;
    public float timer = 0f;
    public float interval = 0.05f;
    private int charIndex = 0;
    private string fullText = "Welcome to Cave Explorer. Your goal is to explore this cave using a Breadth First Search (BFS). (Start with the left node)";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!startScreen.activeInHierarchy && charIndex < fullText.Length) {
            if (timer < interval) {
                timer += Time.deltaTime;
            } else {
                
                paragraph.text += fullText[charIndex].ToString();
                charIndex++;
                timer = 0;
            }
        }
    }
}
