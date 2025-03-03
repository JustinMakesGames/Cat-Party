using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ShopHandler : SpaceHandler
{
    private bool _hasTaskEnded;
    public override async Task HandleAsyncLandedPlayer(Transform player, int currentIndex)
    {
        StartCoroutine(StartShopHandling());
        while (!_hasTaskEnded)
        {
            await Task.Yield();
        }

        _hasTaskEnded = false;      
    }

    private IEnumerator StartShopHandling()
    {
        yield return StartCoroutine(ShowShopText());
    }

    private IEnumerator ShowShopText()
    {
        string shopText = "ShopText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == shopText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings));
    }

    public void ShopDone()
    {
        _hasTaskEnded = true;
    }
}
