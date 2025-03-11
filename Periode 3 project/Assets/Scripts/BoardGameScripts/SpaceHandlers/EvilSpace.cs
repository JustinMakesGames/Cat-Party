using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class EvilSpace : SpaceHandler
{
    public Transform player;
    private bool _hasTaskEnded;
    private bool _isCPU;

    public override IEnumerator HandleLandedPlayer(Transform player)
    {
        this.player = player;

        if (!player.GetComponent<PlayerHandler>().isPlayer)
        {

            _isCPU = true;
        }
        yield return StartCoroutine(EvilText());
        yield return StartCoroutine(ChooseEvent());
        while (!_hasTaskEnded)
        {
            yield return null;
        }

        _hasTaskEnded = false;
    }

    private IEnumerator EvilText()
    {
        string evilText = "EvilText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == evilText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings, _isCPU));
        yield return new WaitForSeconds(1);
    }

    private IEnumerator ChooseEvent()
    {
        int randomEvent = Random.Range(0, 2);

        switch (randomEvent)
        {
            case 0:
                yield return StartCoroutine(StealCoins());
                break;
            case 1:
                yield return StartCoroutine(GiveCoinsToLastPlayer());
                break;
            
        }

        _hasTaskEnded = true;
    }

    private IEnumerator StealCoins()
    {
        int[] possibleCoinAmount = { 10, 20, 30 };

        int coinAmount = Random.Range(0, possibleCoinAmount.Length);
        string evilText = $"Muhahahaha I am gonna steal {possibleCoinAmount[coinAmount]} coins from you and there is nothing you can do about it!";
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(evilText, _isCPU));
        yield return StartCoroutine(CoinChange.Instance.LoseCoins(player, possibleCoinAmount[coinAmount]));

        string closingEvilText = "Okay that was it! See ya around!";
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(closingEvilText, _isCPU));
    }

    private IEnumerator GiveCoinsToLastPlayer()
    {
        int[] possibleCoinAmount = { 5, 10, 15 };

        int coinAmount = Random.Range(0, possibleCoinAmount.Length);
        Transform playerInLast = BoardPlacementManager.Instance.GetPlayerInLast();
        string evilText = $"Muhahahaha you are going to give {possibleCoinAmount[coinAmount]} to the player in last place. I am so evil!";
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(evilText, _isCPU));

        yield return StartCoroutine(CoinChange.Instance.LoseCoins(player, -possibleCoinAmount[coinAmount]));
        yield return new WaitForSeconds(0.5f);
        Camera.main.transform.position = playerInLast.GetChild(1).position;
        yield return StartCoroutine(CoinChange.Instance.WinCoins(playerInLast, possibleCoinAmount[coinAmount]));
        yield return new WaitForSeconds(0.5f);
        Camera.main.transform.position = player.GetChild(1).position;
        yield return new WaitForSeconds(0.5f);
        string closingEvilText = "Okay, that was it! See ya around!";

        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(closingEvilText, _isCPU));

    }
}
