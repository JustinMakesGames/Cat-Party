using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class HandleReachingYarn : MonoBehaviour
{
    [SerializeField] private GameObject buyYarnCanvas;
    [SerializeField] private GameObject coinCanvas;
    [SerializeField] private GameObject coinPrefab;

    private SpaceHandler _spaceHandler;
    private PlayerHandler _playerHandler;
    private MultiplayerEventSystem _eventSystem;
    private bool taskEnded;

    private void Awake()
    {
        _playerHandler = GetComponent<PlayerHandler>();
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }
    public async Task HandleReachingYarnSpace(SpaceHandler handler)
    {
        taskEnded = false;
        _spaceHandler = handler;
        transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
        
        if (_playerHandler.coinAmount < 0)
        {
            StartCoroutine(HandleNotEnoughCoins());
        }

        else
        {
            HandleBuyingYarn();
        }

        while (!taskEnded)
        {
            await Task.Yield();
        }

        transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
    }

    private void HandleBuyingYarn()
    {
        if (_playerHandler.isPlayer)
        {
            StartCoroutine(HandleBuyingYarnPlayer());
        }

        else
        {
            StartCoroutine(HandleBuyingYarnCPU());
        }
    }

    private IEnumerator HandleBuyingYarnPlayer()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt("Would you like to buy the yarn ball?", _playerHandler));

        yield return new WaitForSeconds(0.5f);

        buyYarnCanvas.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(buyYarnCanvas.transform.GetChild(0).gameObject);
    }

    public void PlayerBuysYarn()
    {
        buyYarnCanvas.SetActive(false);
        StartCoroutine(GetYarnBall());
    }

    public void PlayerDoesNotBuyYarn()
    {
        buyYarnCanvas.SetActive(false);
        taskEnded = true;
    }

    private IEnumerator HandleBuyingYarnCPU()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt("Would you like to buy the yarn ball?", true));
        StartCoroutine(GetYarnBall());

    }
    private IEnumerator HandleNotEnoughCoins()
    {
        yield return StartCoroutine(TextListScript.Instance.ShowPrompt("You do not have enough coins to buy a yarn ball"));
        taskEnded = true;
    }

    private IEnumerator GetYarnBall()
    {
        int coinAmount = -20;
        for (int i = 0; i < 3; i++)
        {
            GameObject coinClone = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            Destroy(coinClone);

        }

        GameObject cloneCanvas = Instantiate(coinCanvas, transform.GetChild(0).position, Quaternion.identity);

        Destroy(cloneCanvas, 1);
        cloneCanvas.GetComponentInChildren<TMP_Text>().text = coinAmount.ToString();
        transform.GetComponent<PlayerHandler>().ChangeCoinValue(coinAmount);

        yield return new WaitForSeconds(2);

        
        GameObject cloneYarnBallCanvas = Instantiate(coinCanvas, transform.GetChild(0).position, Quaternion.identity);

        Destroy(cloneYarnBallCanvas, 1);
        cloneYarnBallCanvas.GetComponentInChildren<TMP_Text>().text = "+1";

        _playerHandler.ChangeYarnValue(1);
        yield return new WaitForSeconds(2);

        SetSpaceHandlerToNormal();
        yield return StartCoroutine(YarnPlacement.Instance.StartShowingYarnPlacement());
        StartCoroutine(ReturnToNormal());
    }

    private IEnumerator ReturnToNormal()
    {
        Camera.main.transform.position = transform.GetChild(1).position;
        Camera.main.transform.parent = transform;
        yield return new WaitForSeconds(1);
        taskEnded = true;
    }

    private void SetSpaceHandlerToNormal()
    {
        _spaceHandler.isYarnPlace = false;
        _spaceHandler.transform.GetComponent<Renderer>().enabled = true;

        Destroy(_spaceHandler.transform.GetChild(0).gameObject);
    }
}
