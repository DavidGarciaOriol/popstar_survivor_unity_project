using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonAndroid : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Simula la pulsación de la tecla ESC
        EscapeKeyPress();
    }

    private void EscapeKeyPress()
    {
        GameManager.instance.PauseGame();
        Debug.Log("Escape button pressed");
    }
}