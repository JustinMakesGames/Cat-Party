using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpace : SpaceHandler
{
    public Transform player;
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
        string luckyText = "LuckyText";
        List<string> textStrings = TextListScript.Instance.textStrings.Find(t => t.name == luckyText).strings;
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(textStrings, _isCPU));
    }

    private IEnumerator ChooseEvent()
    {
        yield return null;
    }
}
