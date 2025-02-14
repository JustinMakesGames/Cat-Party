using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class SpaceHandler : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount;
    [SerializeField] private GameObject coinCollectCanvas;
    public enum SpaceKind
    {
        Normal,
        Special
    }

    public SpaceKind spaceKind;

    public bool IsSpaceCountable()
    {
        if (spaceKind == SpaceKind.Normal) return true;

        return false;
    }

    public async virtual Task HandleLandedPlayer(Transform player)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(coinPrefab, player.position, Quaternion.identity);
            await Task.Delay(300);
            Destroy(coinClone);
            
        }

        GameObject cloneCanvas = Instantiate(coinCollectCanvas, player.GetChild(0).position, Quaternion.identity);

        cloneCanvas.GetComponentInChildren<TMP_Text>().text = coinAmount >= 0 ? "+" + coinAmount.ToString() : coinAmount.ToString();
        player.GetComponent<PlayerHandler>().ChangeCoinValue(coinAmount);
        await Task.Delay(1000);
        Destroy(cloneCanvas);
    }
}
