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
    [SerializeField] private Transform cancelItemScreen;
    [SerializeField] private GameObject cancelInventoryScreen;
    [SerializeField] private GameObject emptyUI;
    private MultiplayerEventSystem _eventSystem;
    private bool _hasOpenedMenu;

    public bool hasCancelledItem;


    private void Awake()
    {
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public IEnumerator AddItem(GameObject item)
    {
        GameObject itemClone = Instantiate(item, itemFolder);
        itemClone.GetComponent<Item>().player = transform;
        items.Add(itemClone);
        GameObject cancelItemClone = Instantiate(emptyUI, cancelItemScreen);
        cancelItemClone.GetComponent<ItemReference>().item = itemClone;
        cancelItemClone.GetComponent<ItemReference>().inventory = this;
        if (items.Count > 1)
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
        string prompt = "You got too much items, which one would you like to get rid off";

        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(prompt));

        if (GetComponent<PlayerHandler>().isPlayer)
        {
            cancelInventoryScreen.SetActive(true);

            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(cancelItemScreen.GetChild(0).gameObject);
        }

        else
        {
            int randomItem = Random.Range(0, cancelItemScreen.childCount);

            cancelItemScreen.GetChild(randomItem).GetComponent<ItemReference>().RemovethisItem();
        }
        

        while (!hasCancelledItem)
        {
            yield return null;
        }

        hasCancelledItem = false;
    }
}
