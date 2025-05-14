using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class GetAlgo : MonoBehaviour
{
    public TMP_Dropdown algorithmDropdown; // Reference to the TMP_Dropdown menu
    public TreeLogic treeLogic; // Reference to the TreeLogic script

    private void Start()
    {
        if (algorithmDropdown != null)
        {
            // Add a listener to handle dropdown value changes
            algorithmDropdown.onValueChanged.AddListener(OnAlgorithmChanged);
        }
        else
        {
            Debug.LogError("Algorithm dropdown is not assigned!");
        }
    }

    // Called when the dropdown value changes
    private void OnAlgorithmChanged(int value)
    {
        if (treeLogic != null)
        {
            // Update the algorithm in TreeLogic based on the dropdown value
            switch (value)
            {
                case 0:
                    treeLogic.SetAlgorithm("BFS");
                    Debug.Log("Algorithm set to BFS");
                    break;
                case 1:
                    treeLogic.SetAlgorithm("DFS");
                    Debug.Log("Algorithm set to DFS");
                    break;
                default:
                    Debug.LogError("Invalid dropdown value!");
                    break;
            }
        }
    }
}
