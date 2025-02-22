using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class SpaceHandler : MonoBehaviour
{
    public bool isYarnPlace;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinAmount;
    [SerializeField] private GameObject coinCollectCanvas;
    public enum SpaceKind
    {
        Normal,
        Special,
        YarnPlace
    }

    public SpaceKind spaceKind;

    public bool IsSpaceCountable()
    {
        if (spaceKind == SpaceKind.Normal) return true;

        return false;
    }

    public void SetSpaceAsYarnSpace()
    {
        isYarnPlace = true;
    }

    public virtual IEnumerator HandleLandedPlayer(Transform player)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(coinPrefab, player.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(coinClone);
            
        }

        GameObject cloneCanvas = Instantiate(coinCollectCanvas, player.GetChild(0).position, Quaternion.identity);

        cloneCanvas.GetComponentInChildren<TMP_Text>().text = coinAmount >= 0 ? "+" + coinAmount.ToString() : coinAmount.ToString();
        player.GetComponent<PlayerHandler>().ChangeCoinValue(coinAmount);
        yield return new WaitForSeconds(1);
        Destroy(cloneCanvas);
    }
}
