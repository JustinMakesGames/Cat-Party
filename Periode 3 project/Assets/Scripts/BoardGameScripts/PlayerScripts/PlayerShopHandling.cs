using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class PlayerShopHandling : MonoBehaviour
{
    [SerializeField] private GameObject chooseScreen;
    [SerializeField] private Transform shopScreen;
    private MultiplayerEventSystem _eventSystem;
    private ShopHandler _shopHandler;

    private void Awake()
    {
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public void StartShopping(ShopHandler shopHandler)
    {
        _shopHandler = shopHandler;
        chooseScreen.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(chooseScreen.transform.GetChild(0).gameObject);
    }

    public void PlayerChoosesNotToBuy()
    {
        _shopHandler.ShopDone();
    }


}
