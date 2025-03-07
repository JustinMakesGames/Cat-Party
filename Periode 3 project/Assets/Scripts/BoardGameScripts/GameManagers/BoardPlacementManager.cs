using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
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
    public void ShowBoardPlacement(PlayerHandler playerHandler)
    {      
        
        StartCoroutine(ShowBoardPlacementAnimation(playerHandler));
    }

    private IEnumerator ShowBoardPlacementAnimation(PlayerHandler playerHandler)
    {
        print($"This is the playerhandler of today: {playerHandler.name}");
        yield return new WaitForSeconds(1);
        playerUIFolder.gameObject.SetActive(true);
        CalculateUIOrder();
        yield return new WaitForSeconds(2);
        GameObject playerUITransform = null;
        foreach (PlayerUIStats stats in _playerUIStats)
        {
            print($"{stats.playerHandler.name} vs {playerHandler.name}");
            if (stats.playerHandler == playerHandler)
            {
                playerUITransform = stats.playerTransform.GetChild(0).gameObject;
                playerUITransform.SetActive(true);
                break;
            }
        }
        yield return new WaitForSeconds(2);
        playerUITransform.SetActive(false);
        playerHandler.ChangeCoinValue(10);
        CalculateUIOrder();
        yield return new WaitForSeconds(2);

        playerUIFolder.gameObject.SetActive(false);

        StartCoroutine(BoardGameManager.Instance.StartNewTurn());

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

        for (int i = 0; i < orderedStats.Count; i++)
        {
            orderedStats[i].playerTransform.SetSiblingIndex(i);
        }


    }
}
