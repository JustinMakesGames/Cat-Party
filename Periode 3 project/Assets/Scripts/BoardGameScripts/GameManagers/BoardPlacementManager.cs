using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class PlayerUIStats
{
    public PlayerHandler playerHandler;
    public Transform playerTransform;
    public TMP_Text yarnAmount;
    public TMP_Text coinAmount;

    public PlayerUIStats(PlayerHandler playerHandler, Transform playerTransform, TMP_Text yarnAmount, TMP_Text coinAmount)
    {
        this.playerHandler = playerHandler;
        this.playerTransform = playerTransform;
        this.yarnAmount = yarnAmount;
        this.coinAmount = coinAmount;
    }
}

public class BoardPlacementManager : MonoBehaviour
{
    public static BoardPlacementManager Instance;
    [SerializeField] private Transform playerFolder;
    [SerializeField] private Transform playerUIFolder;
    [SerializeField] private GameObject normalPlayerUI;
    private List<PlayerHandler> _playerHandlers = new List<PlayerHandler>();
    private List<PlayerUIStats> _playerUIStats = new List<PlayerUIStats>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        MakePlayerUIStats();


    }

    public Transform GetPlayerInLast(Transform currentPlayer)
    {
        CalculateUIOrder();

        if (_playerUIStats[_playerUIStats.Count - 1].playerHandler.transform == currentPlayer)
        {
            return _playerUIStats[_playerUIStats.Count - 2].playerHandler.transform;
        }
        return _playerUIStats[_playerUIStats.Count - 1].playerHandler.transform;
    }

    private void MakePlayerUIStats()
    {
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            _playerHandlers.Add(playerFolder.GetChild(i).GetComponent<PlayerHandler>());
        }

        for (int i = 0; i < _playerHandlers.Count; i++)
        {
            PlayerUIStats stats = new PlayerUIStats(playerFolder.GetChild(i).GetComponent<PlayerHandler>(), playerUIFolder.GetChild(i),
                playerUIFolder.GetChild(i).GetChild(1).GetComponent<TMP_Text>(),
                playerUIFolder.GetChild(i).GetChild(2).GetComponent<TMP_Text>());
            
            _playerUIStats.Add(stats);
        }
    }
    public void ShowBoardPlacement(Dictionary<PlayerHandler, int> playerHandler)
    {      
        
        StartCoroutine(ShowBoardPlacementAnimation(playerHandler));
    }

    private IEnumerator ShowBoardPlacementAnimation(Dictionary<PlayerHandler, int> playerHandler)
    {
        yield return new WaitForSeconds(1);
        playerUIFolder.gameObject.SetActive(true);
        CalculateUIOrder();
        yield return new WaitForSeconds(2);

        foreach (KeyValuePair<PlayerHandler, int> kvp in playerHandler)
        {
            foreach (PlayerUIStats stats in _playerUIStats)
            {
                if (stats.playerHandler == kvp.Key)
                {
                    CalculatePlacement(kvp.Key, kvp.Value);
                }
            }
        }
        
        yield return new WaitForSeconds(2);

        foreach (PlayerUIStats stats in _playerUIStats)
        {
            stats.playerTransform.GetChild(0).gameObject.SetActive(false);
        }
        CalculateUIOrder();
        yield return new WaitForSeconds(2);

        playerUIFolder.gameObject.SetActive(false);

        normalPlayerUI.SetActive(true);
        playerHandler.Clear();
        StartCoroutine(BoardGameManager.Instance.StartNewTurn());

    }

    private void CalculatePlacement(PlayerHandler playerHandler, int place)
    {
        int coinValue = 0;
        bool hasWonCoins = false;
        switch (place)
        {
            case 1:
                coinValue = 10;
                playerHandler.ChangeCoinValue(coinValue);
                hasWonCoins = true;
                break;
            case 2:
                coinValue = 6;
                playerHandler.ChangeCoinValue(coinValue);
                hasWonCoins = true;
                break;
            case 3:
                coinValue = 3;
                playerHandler.ChangeCoinValue(coinValue);
                hasWonCoins = true;
                break;
        }

        if (hasWonCoins)
        {
            foreach (PlayerUIStats uiStats in _playerUIStats)
            {
                if (uiStats.playerHandler == playerHandler)
                {
                    uiStats.playerTransform.GetChild(0).gameObject.SetActive(true);
                    uiStats.playerTransform.GetChild(0).GetComponent<TMP_Text>().text = "+" + coinValue.ToString();
                }
            }
        }
    }

    private void CalculateUIOrder()
    {

        foreach (PlayerUIStats stats in _playerUIStats)
        {
            stats.yarnAmount.text = "x" + stats.playerHandler.yarnAmount.ToString();
            stats.coinAmount.text = "x" + stats.playerHandler.coinAmount.ToString();
        }
        var orderedStats = _playerUIStats
        .OrderByDescending(stats => stats.playerHandler.yarnAmount)
        .ThenByDescending(stats => stats.playerHandler.coinAmount)
        .ToList();

        _playerUIStats = orderedStats;

        for (int i = 0; i < orderedStats.Count; i++)
        {
            orderedStats[i].playerTransform.SetSiblingIndex(i);
        }


    }
}
