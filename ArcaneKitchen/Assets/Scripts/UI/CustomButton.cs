using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CustomButton : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public Sprite normalSprite;
    public Sprite highlightedSprite;
    public Sprite pressedSprite;

    private Image buttonImage;
    private bool pointerOver = false;
    private bool pointerDown = false;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
            Debug.LogError("[CustomButton] No se encontró Image en el GameObject.");

        if (normalSprite == null)
            Debug.LogWarning("[CustomButton] normalSprite no asignado en el inspector.");
        if (highlightedSprite == null)
            Debug.LogWarning("[CustomButton] highlightedSprite no asignado en el inspector.");
        if (pressedSprite == null)
            Debug.LogWarning("[CustomButton] pressedSprite no asignado en el inspector.");
    }

    void Start()
    {
        // Inicializa con normal (si existe)
        if (buttonImage != null && normalSprite != null)
            buttonImage.sprite = normalSprite;

        // Si tenés un componente Button en el mismo objeto, puede estar interfiriendo.
        var btn = GetComponent<Button>();
        if (btn != null && btn.transition != Selectable.Transition.None)
        {
            Debug.Log("[CustomButton] Atención: hay un componente Button con Transition distinto de None. Puede sobrescribir el sprite.");
            // Si querés, descomentá la línea siguiente para desactivar la transición del Button:
            // btn.transition = Selectable.Transition.None;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
        if (!pointerDown)
            SetSpriteOrFallback(highlightedSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        if (!pointerDown)
            SetSpriteOrFallback(normalSprite);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        SetSpriteOrFallback(pressedSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        // Si el puntero sigue sobre el botón, volver a highlighted; si no, normal
        if (pointerOver)
            SetSpriteOrFallback(highlightedSprite);
        else
            SetSpriteOrFallback(normalSprite);
    }

    // Para soporte de teclado/gamepad (selección)
    public void OnSelect(BaseEventData eventData)
    {
        SetSpriteOrFallback(highlightedSprite);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetSpriteOrFallback(normalSprite);
    }

    private void SetSpriteOrFallback(Sprite s)
    {
        if (buttonImage == null) return;
        if (s != null)
            buttonImage.sprite = s;
        else if (normalSprite != null)
            buttonImage.sprite = normalSprite; // fallback al normal si falta el solicitado
    }
}
