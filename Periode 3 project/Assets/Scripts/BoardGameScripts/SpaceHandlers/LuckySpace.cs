using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LuckySpace : SpaceHandler
{
    public Transform player;
    [SerializeField] private GameObject canvas;
    [SerializeField] private List<GameObject> images = new List<GameObject>();
    [SerializeField] private List<GameObject> randomItems = new List<GameObject>();
    private GameObject _spawnedItem;
    private bool _isCPU;

    

    public override IEnumerator HandleLandedPlayer(Transform player)
    {
        this.player = player;

        if (!player.GetComponent<PlayerHandler>().isPlayer)
        {
            _isCPU = true;
        }

        yield return StartCoroutine(LuckyText());
        yield return StartCoroutine(ChooseEvent());
    }

    private IEnumerator LuckyText()
    {
        string luckyText = "Lucky Space!";
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(luckyText, true));
        
    }

   

    private IEnumerator ChooseEvent()
    {
        canvas.SetActive(true);
        foreach (GameObject img in images)
        {
            img.SetActive(false);
        }
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < images.Count; j++)
            {
                images[j].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                images[j].gameObject.SetActive(false);
            }
        }

        foreach (GameObject img in images)
        {
            img.SetActive(false);
        }

        int randomEvent = 1;
        images[randomEvent].SetActive(true);

        yield return new WaitForSeconds(1.5f);
        foreach (GameObject img in images)
        {
            img.SetActive(true);
        }
        canvas.SetActive(false);

        switch (randomEvent)
        {
            case 0:
                yield return StartCoroutine(GetFish());
                break;
            case 1:
                yield return StartCoroutine(GetRandomItem());
                break;
        }
    }

    private IEnumerator GetFish()
    {
        int[] possibleCoinAmount = { 10, 20, 30 };

        int coinAmount = Random.Range(0, possibleCoinAmount.Length);
        yield return StartCoroutine(CoinChange.Instance.WinCoins(player, possibleCoinAmount[coinAmount]));
        yield return null;
    }

    private IEnumerator GetRandomItem()
    {
        int randomItem = Random.Range(0, randomItems.Count);
        _spawnedItem = Instantiate(randomItems[randomItem]);
        GameObject clone = Instantiate(_spawnedItem.GetComponent<Item>().animationPrefab, player.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Destroy(clone);
        yield return StartCoroutine(ShowTextItemGot());
        player.GetComponent<PlayerInventory>().AddItem(_spawnedItem.GetComponent<Item>().uiPrefab);
        Destroy(_spawnedItem);
    }

    private IEnumerator ShowTextItemGot()
    {
        string showText = "You got a " + _spawnedItem.GetComponent<Item>().itemName;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(showText, true));
    }
}
