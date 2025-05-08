using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Vector2 lastMousePosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Called when the user clicks on the panel
    public void OnPointerDown(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    // Called when the user drags the panel
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 delta = currentMousePosition - lastMousePosition;

        // Move the panel by the delta
        rectTransform.anchoredPosition += delta;

        // Update the last mouse position
        lastMousePosition = currentMousePosition;
    }
}