using UnityEngine;
using UnityEngine.EventSystems;

public class IsCursorOverUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.CursorManagerInstance.ToggleUiCursor(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.CursorManagerInstance.ToggleUiCursor(false);
    }
}
