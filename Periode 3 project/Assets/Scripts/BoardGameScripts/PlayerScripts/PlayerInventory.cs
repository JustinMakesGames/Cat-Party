using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    [SerializeField] private Transform itemFolder;
    [SerializeField] private GameObject chooseScreen;
    [SerializeField] private GameObject inventoryScreen;
    private MultiplayerEventSystem _eventSystem;
    private bool _hasOpenedMenu;


    private void Awake()
    {
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public IEnumerator AddItem(GameObject item)
    {
        GameObject itemClone = Instantiate(item, itemFolder);
        itemClone.GetComponent<Item>().player = transform;
        items.Add(itemClone);
        if (items.Count > 3)
        {
            yield return StartCoroutine(HandleTooMuchItems());
        }
        

    }

    public void OpenItemMenu()
    {
        if (items.Count > 0)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(itemFolder.GetComponentInChildren<Button>().gameObject);
            
        }

        _hasOpenedMenu = true;

    }

    public void CloseItemMenu(InputAction.CallbackContext context)
    {
        if (context.performed && _hasOpenedMenu)
        {
            OpenChooseScreen();
        }
    }

    public void OpenChooseScreen()
    {
        inventoryScreen.SetActive(false);
        chooseScreen.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(chooseScreen.transform.GetChild(0).GetChild(0).gameObject);
        _hasOpenedMenu = false;
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

    private IEnumerator HandleTooMuchItems()
    {

    }
}
