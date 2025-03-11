using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinChange : MonoBehaviour
{
    public static CoinChange Instance;
    public GameObject winCoinPrefab, loseCoinPrefab;
    public GameObject coinCollectCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public IEnumerator WinCoins(Transform player, int coinAmount)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(winCoinPrefab, player.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(coinClone);

        }

        GameObject cloneCanvas = Instantiate(coinCollectCanvas, player.GetChild(0).position, Quaternion.identity);

        cloneCanvas.GetComponentInChildren<TMP_Text>().text = coinAmount >= 0 ? "+" + coinAmount.ToString() : coinAmount.ToString();
        player.GetComponent<PlayerHandler>().ChangeCoinValue(coinAmount);
        yield return new WaitForSeconds(1);
        Destroy(cloneCanvas);
    }

    public IEnumerator LoseCoins(Transform player, int coinAmount)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(loseCoinPrefab, player.position, Quaternion.identity);
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
