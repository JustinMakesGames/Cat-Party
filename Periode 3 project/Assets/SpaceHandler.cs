using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;

public class SpaceHandler : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount;
    [SerializeField] private GameObject textCoinAmount;
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

        

        player.GetComponent<PlayerHandler>().coinAmount += coinAmount;
        await Task.Delay(1000);
    }
}
