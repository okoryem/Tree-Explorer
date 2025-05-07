using UnityEngine;
using UnityEngine.UI;

public class Jewel3 : MonoBehaviour
{
    public Text jewel1Text;
    public int jewel3Count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Increase Jewel 3")]
    public void addJewel3(int amount) {
        jewel3Count += amount;
        jewel1Text.text = jewel3Count.ToString();

    }
}
