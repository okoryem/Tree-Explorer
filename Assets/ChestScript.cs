using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool canInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if (!canInteract()) return;
        OpenChest();
    }

    private void OpenChest() {
        setOpened(true);

        //Drop Item
        if (itemPrefab) {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
        }
    }

    public void setOpened(bool opened) {
        IsOpened = opened;
        if (IsOpened) {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
