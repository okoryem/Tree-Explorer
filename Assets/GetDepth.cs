using UnityEngine;
using UnityEngine.UI;

public class GetDepth : MonoBehaviour
{
    public Slider depthSlider; // Reference to the slider
    public TreeLogic treeLogic; // Reference to the TreeLogic script
    public Text depthText; // Reference to the text box to display the depth

    private void Start()
    {
        if (depthSlider != null)
        {
            // Add a listener to handle slider value changes
            depthSlider.onValueChanged.AddListener(OnDepthChanged);

            // Set the slider's default value to match the TreeLogic's depth
            depthSlider.value = treeLogic != null ? treeLogic.depth : 3;

            // Update the text box with the initial depth value
            UpdateDepthText(depthSlider.value);
        }
        else
        {
            Debug.LogError("Depth slider is not assigned!");
        }

        if (treeLogic == null)
        {
            Debug.LogError("TreeLogic script is not assigned!");
        }

        if (depthText == null)
        {
            Debug.LogError("Depth text box is not assigned!");
        }
    }

    // Called when the slider value changes
    private void OnDepthChanged(float value)
    {
        if (treeLogic != null)
        {
            int newDepth = Mathf.RoundToInt(value); // Convert the slider value to an integer
            treeLogic.SetDepth(newDepth);
        }

        // Update the text box with the new depth value
        UpdateDepthText(value);
    }

    // Updates the text box to display the current depth
    private void UpdateDepthText(float value)
    {
        if (depthText != null)
        {
            depthText.text = $"{Mathf.RoundToInt(value)}";
        }
    }
}
