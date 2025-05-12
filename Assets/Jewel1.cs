using UnityEngine;
using UnityEngine.UI;

public class Jewel1 : MonoBehaviour
{
    public Text jewel1Text;
    public int jewel1Count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Increase Jewel 1")]
    public void addJewel1(int amount) {
        jewel1Count += amount;
        jewel1Text.text = jewel1Count.ToString();
        
    }
}
