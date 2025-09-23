using UnityEngine;
using UnityEngine.UI;
public class SimpleInventory : MonoBehaviour
{
    public Image[] slots;            // tus 3 cuadrados negros en UI
    public Color emptyColor = Color.black;
    public Color highlightColor = Color.yellow; // slot seleccionado
    public int selectedSlot = 0;

    private GameObject[] items;

    void Start()
    {
        items = new GameObject[slots.Length];

        // Inicializamos slots vacíos
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = null;
            slots[i].color = emptyColor;
            items[i] = null;
        }

        UpdateSlotHighlights();
    }

    void Update()
    {
        // Cambiar slot con rueda del mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SelectNextSlot();
        else if (scroll < 0f) SelectPreviousSlot();

        UpdateSlotHighlights();

        // Test: soltar el objeto del slot seleccionado con tecla Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Drop(selectedSlot);
        }
    }

    // Agarra objeto y lo guarda en el primer slot vacío
    public void PickUp(GameObject obj, Sprite objSprite)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = obj;
                slots[i].sprite = objSprite;
                slots[i].color = (i == selectedSlot) ? highlightColor : Color.white;
                obj.SetActive(false);
                break;
            }
        }
    }

    // Suelta el objeto del slot indicado
    public void Drop(int slotIndex)
    {
        if (items[slotIndex] != null)
        {
            items[slotIndex].SetActive(true);
            items[slotIndex].transform.position = transform.position + transform.forward;
            items[slotIndex] = null;
            slots[slotIndex].sprite = null;
            slots[slotIndex].color = emptyColor;
        }
    }

    // Mover selección
    void SelectNextSlot()
    {
        selectedSlot++;
        if (selectedSlot >= slots.Length)
            selectedSlot = 0;
    }

    void SelectPreviousSlot()
    {
        selectedSlot--;
        if (selectedSlot < 0)
            selectedSlot = slots.Length - 1;
    }

    // Resaltar slots
    void UpdateSlotHighlights()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == selectedSlot)
                slots[i].color = highlightColor;
            else if (items[i] == null)
                slots[i].color = emptyColor;
            else
                slots[i].color = Color.white;
        }
    }
}
