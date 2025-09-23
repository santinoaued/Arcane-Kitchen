using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Sprite itemSprite; // imagen del objeto para el UI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleInventory inventory = other.GetComponent<SimpleInventory>();
            if (inventory != null)
            {
                inventory.PickUp(gameObject, itemSprite);
            }
        }
    }
}
