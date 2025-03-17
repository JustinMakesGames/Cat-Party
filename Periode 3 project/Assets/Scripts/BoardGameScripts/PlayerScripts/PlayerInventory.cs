using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    [SerializeField] private Transform itemFolder;
    private MultiplayerEventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public void AddItem(GameObject item)
    {
        items.Add(item);
        GameObject itemClone = Instantiate(item, itemFolder);
        itemClone.GetComponent<Item>().player = transform;

    }

    public void OpenItemMenu()
    {
        if (items.Count > 0)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(itemFolder.GetComponentInChildren<Button>().gameObject);
        }
        
    }

    public void UseItemCPU()
    {
        itemFolder.parent.gameObject.SetActive(true);
        items[0].GetComponent<Item>().StartUsingItem();
    }

    public void RemoveItem(GameObject item)
    {
        items.Remove(item);
    }
}
