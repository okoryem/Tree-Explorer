using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;
    public AudioSource OpenChestSFX;

    private TreeLogic treeLogic; // Reference to TreeLogic
    private Tutorial tutorial; // Reference to the Tutorial script

    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);

        // Find the TreeLogic component
        GameObject treeLogicObject = GameObject.FindGameObjectWithTag("TreeLogic");
        if (treeLogicObject != null)
        {
            treeLogic = treeLogicObject.GetComponent<TreeLogic>();
        }

        if (treeLogic == null)
        {
            Debug.LogError("TreeLogic component not found! Ensure the Tree Logic GameObject is tagged with 'TreeLogic' and has the TreeLogic script attached.");
        }

        // Find the Tutorial component
        GameObject tutorialObject = GameObject.FindGameObjectWithTag("Tutorial");
        if (tutorialObject != null)
        {
            tutorial = tutorialObject.GetComponent<Tutorial>();
        }

        if (tutorial == null)
        {
            Debug.LogError("Tutorial component not found! Ensure the Tutorial GameObject is tagged with 'Tutorial' and has the Tutorial script attached.");
        }
    }

    public bool canInteract()
    {
        // Check if the chest is already opened
        if (IsOpened) return false;

        // Check if the current cave node is visited
        TreeLogic.TreeStructure.Node currentNode = treeLogic.FindNodeByGameObject(transform.parent.gameObject);
        if (currentNode != null && treeLogic.visitedNodes.Contains(currentNode))
        {
            return true; // Chest can be opened
        }

        Debug.Log("Chest cannot be opened because the cave node has not been visited.");
        return false;
    }

    public void Interact()
    {
        if (!canInteract()) return;
        OpenChest();
    }

    private void OpenChest()
    {
        setOpened(true);
        OpenChestSFX.Play();

        // Drop Items
        if (itemPrefab)
        {
            // Drop one item directly below
            Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);

            // Drop one item down and to the left
            Instantiate(itemPrefab, transform.position + Vector3.down + Vector3.left * 0.5f, Quaternion.identity);

            // Drop one item down and to the right
            Instantiate(itemPrefab, transform.position + Vector3.down + Vector3.right * 0.5f, Quaternion.identity);
        }

        // Trigger the inventory tutorial
        if (tutorial != null)
        {
            tutorial.showInventory();
        }
    }

    public void setOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
