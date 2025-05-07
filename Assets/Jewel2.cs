using UnityEngine;
using UnityEngine.UI;

public class Jewel2 : MonoBehaviour
{
    public Text jewel2Text;
    public int jewel2Count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Increase Jewel 2")]
    public void addJewel2(int amount) {
        jewel2Count += amount;
        jewel2Text.text = jewel2Count.ToString();

    }
}
