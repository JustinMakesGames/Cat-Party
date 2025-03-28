using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerShopHandling : MonoBehaviour
{
    public Vector3 minScale;
    public Vector3 maxScale;
    public float speed;
    [SerializeField] private GameObject chooseScreen;
    [SerializeField] private Transform shopScreen;
    private MultiplayerEventSystem _eventSystem;
    private bool _isScalingUp;
    private ShopHandler _shopHandler;
    private Transform _itemSelectionFolder;
    private Transform selectedItem;

    private TMP_Text itemDescription;
    private bool _isShopping;

    private void Awake()
    {
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
        itemDescription = GameObject.FindGameObjectWithTag("GameCanvas").transform.GetComponentInChildren<TMP_Text>();
    }

    public void AskIfPlayerWantsToShop(ShopHandler shopHandler)
    {
        _shopHandler = shopHandler;
        chooseScreen.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(chooseScreen.transform.GetChild(0).gameObject);
    }

    public void PlayerChoosesToShop()
    {
        StartCoroutine(_shopHandler.MoveCameraToShop());
    }

    public void PlayerChoosesNotToBuy()
    {
        _shopHandler.ShopDone();
    }

    public void BeginShopping(Transform selectedItem)
    {
        this.selectedItem = selectedItem;
        _isShopping = true;
        StartCoroutine(HandleSelectedItem());
    }

    private IEnumerator HandleSelectedItem()
    {
        GameObject.FindGameObjectWithTag("GameCanvas").transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        while (_isShopping)
        {
            ScaleSelectedItem();
            ShowItemDescription();
            yield return null;
        }
    }
    private void ScaleSelectedItem()
    {
        Vector3 targetScale = _isScalingUp ? maxScale : minScale;

        selectedItem.localScale = Vector3.MoveTowards(selectedItem.localScale, targetScale, Time.deltaTime * speed);

        if (Vector3.Distance(selectedItem.localScale, targetScale) < 0.05f)
        {
            _isScalingUp = !_isScalingUp;
        }
    }

    private void ShowItemDescription()
    {
        itemDescription.text = selectedItem.GetComponent<Item>().itemDescription;
    }

    public void MoveSelectedItem(InputAction.CallbackContext context)
    {
        if (context.started && _isShopping && context.ReadValue<Vector2>().x > 0)
        {
            selectedItem.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            int selectedIndex = selectedItem.GetSiblingIndex();

            if (selectedIndex == selectedItem.parent.childCount - 1)
            {
                selectedIndex = -1;
            }

            selectedItem = selectedItem.parent.GetChild(selectedIndex + 1);
        }

        if (context.started && _isShopping && context.ReadValue<Vector2>().x < 0)
        {
            selectedItem.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            int selectedIndex = selectedItem.GetSiblingIndex();

            if (selectedIndex == 0)
            {
                selectedIndex = selectedItem.parent.childCount;
            }
            selectedItem = selectedItem.parent.GetChild(selectedIndex - 1);
        }
    }

    public void SelectItem(InputAction.CallbackContext context)
    {
        if (context.performed && _isShopping)
        {
            GameObject.FindGameObjectWithTag("GameCanvas").transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            BuyItem();
        }
    }

    public void CancelShop(InputAction.CallbackContext context)
    {
        if (context.performed && _isShopping)
        {
            GameObject.FindGameObjectWithTag("GameCanvas").transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            Camera.main.transform.position = transform.GetChild(1).position;
            Camera.main.transform.rotation = transform.GetChild(1).rotation;
            _isShopping = false;
            _shopHandler.ShopDone();
        }
    }
    private void BuyItem()
    {
        if (GetComponent<PlayerHandler>().coinAmount < selectedItem.GetComponent<Item>().price)
        {
            HandleNotEnoughMoney();
            return;
        }

        _isShopping = false;

        StartCoroutine(_shopHandler.HandlePlayerBuyingItem(selectedItem.GetComponent<Item>()));
    }

    private void HandleNotEnoughMoney()
    {
        string poorText = "I am sorry, you do not have enough money to buy this item.";
        StartCoroutine(TextListScript.Instance.ShowPrompt(poorText, true));
    }


}
