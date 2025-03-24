using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReference : MonoBehaviour
{
    public PlayerInventory inventory;
    public GameObject item;

    public void RemovethisItem()
    {
        inventory.RemoveItem(item);
        Destroy(item);
        transform.parent.parent.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
