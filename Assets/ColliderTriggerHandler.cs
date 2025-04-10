using UnityEngine;

public class ColliderTriggerHandler : MonoBehaviour
{
    public System.Action<GameObject> OnTriggerEnterAction; // Action to invoke on trigger enter

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) // Assuming layer 6 is the miner
        {
            OnTriggerEnterAction?.Invoke(collision.gameObject);
        }
    }
}