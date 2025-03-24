using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopHandler : SpaceHandler
{
    [SerializeField] private float camSpeed; 
    [SerializeField] private Transform camPosition;
    [SerializeField] private Transform itemFolder;

    private Item _boughtItem;
    private Transform _player;
    private bool _hasTaskEnded;
    private Transform _cam;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        _player = player;
        _cam = Camera.main.transform;
        _player.GetChild(_player.childCount - 1).gameObject.SetActive(false);

        if (player.GetComponent<PlayerHandler>().isPlayer)
        {
            StartCoroutine(StartPlayerShopping());
        }
        
        else
        {
            StartCoroutine(StartCPUShopping());
        }
        while (!_hasTaskEnded)
        {
            await Task.Yield();
        }

        _hasTaskEnded = false;      
    }

    private IEnumerator StartPlayerShopping()
    {
        
        yield return StartCoroutine(ShowShopTextPlayer());
        _player.GetComponent<PlayerShopHandling>().AskIfPlayerWantsToShop(this);    
    }

    private IEnumerator StartCPUShopping()
    {
        yield return StartCoroutine(ShowShopTextCPU());

        Item returnItem = ChoosePossibleItems();

        if (returnItem == null)
        {
            ShopDone();
            yield break;
        }
        StartCoroutine(HandlePlayerBuyingItem(returnItem));
    }

    private Item ChoosePossibleItems()
    {
        List<Item> possibleItems = new List<Item>();
        PlayerHandler playerHandler = _player.GetComponent<PlayerHandler>();
        for (int i = 0; i < itemFolder.childCount; i++)
        {
             Item item = itemFolder.GetChild(i).GetComponent<Item>();
             
            if (playerHandler.coinAmount >= item.price)
            {
                possibleItems.Add(item);
            } 
        }

        if (possibleItems.Count > 0)
        {
            int randomItem = Random.Range(0, possibleItems.Count);

            return possibleItems[randomItem];
        }

        return null;
    }
    private IEnumerator ShowShopTextPlayer()
    {
        string shopText = "ShopText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == shopText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));
    }

    private IEnumerator ShowShopTextCPU()
    {
        string shopText = "ShopText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == shopText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings, true));
    }
 
    public IEnumerator MoveCameraToShop()
    {
        _cam.rotation = camPosition.rotation;
        while (_cam.position != camPosition.position)
        {
            _cam.position = Vector3.MoveTowards(_cam.position, camPosition.position, camSpeed * Time.deltaTime);
            yield return null;
            
        }
        _player.GetComponent<PlayerShopHandling>().BeginShopping(itemFolder.GetChild(0));
    }

    public IEnumerator HandlePlayerBuyingItem(Item item)
    {
        _boughtItem = item;
        SetCameraBack();
        yield return StartCoroutine(PlayGetItemAnimation());
    }

    private IEnumerator PlayGetItemAnimation()
    {
        int coinAmount = -_boughtItem.price;
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(coinPrefab, _player.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(coinClone);

        }

        _player.GetComponent<PlayerHandler>().ChangeCoinValue(coinAmount);

        GameObject cloneCanvas = Instantiate(coinCollectCanvas, _player.GetChild(0).position, Quaternion.identity);
        cloneCanvas.GetComponentInChildren<TMP_Text>().text = coinAmount >= 0 ? "+" + coinAmount.ToString() : coinAmount.ToString();
        yield return new WaitForSeconds(1);
        Destroy(cloneCanvas);
        GameObject clone = Instantiate(_boughtItem.animationPrefab, _player.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Destroy(clone);
        yield return StartCoroutine(ShowTextItemGot());
        yield return StartCoroutine(_player.GetComponent<PlayerInventory>().AddItem(_boughtItem.uiPrefab));
        ShopDone();

    }

    private IEnumerator ShowTextItemGot()
    {
        string showText = "You got a " + _boughtItem.itemName;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(showText, true));
    }

    private void SetCameraBack()
    {
        _cam.position = _player.GetChild(1).position;
        _cam.rotation = _player.GetChild(1).rotation;
    }
    
    public void ShopDone()
    {
        _player.GetChild(_player.childCount - 1).gameObject.SetActive(true);
        _hasTaskEnded = true;
    }
}
