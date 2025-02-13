using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public Transform player;
    public PlayerHandler handler;
    public int Coins
    {
        get => handler.coinAmount;
        set => handler.coinAmount = value;
    }

    public int WoolAmount
    {
        get => handler.woolAmount;
        set => handler.woolAmount = value;
    }

    public int CurrentPlacement
    {
        get => handler.currentSpace;
        set => handler.currentSpace = value;
    }

    public PlayerStats(Transform player, PlayerHandler handler)
    {
        this.player = player;
        this.handler = handler;
    }
}
public class BoardGameManager : MonoBehaviour
{
    public static BoardGameManager Instance;
    public enum BoardStates 
    {
        Start,
        TurnOfAPlayer,
        MinigameTime

    }

    public BoardStates state;
    [SerializeField] private Transform playerFolder;
    [SerializeField] private GameObject blackScreen;
    
    public List<Transform> playerTransforms = new List<Transform>();
    public List<PlayerStats> players = new List<PlayerStats>();

    private int _playerIndex = -1;
    private bool _gameHasStarted;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        AssignPlayers();
    }
    
    private void AssignPlayers()
    {
        playerFolder = GameObject.FindGameObjectWithTag("PlayerFolder").transform;
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            playerTransforms.Add(playerFolder.GetChild(i));
        }
    }

    public void MakeTheTurnOrder()
    {
        playerTransforms.Sort((a, b) => {
            int numberA = a.GetComponent<PlayerHandleStart>().rolledNumber;
            int numberB = b.GetComponent<PlayerHandleStart>().rolledNumber;

            return numberB.CompareTo(numberA);
        });

        AssignPlayerStats();
    }

    private void AssignPlayerStats()
    {
        for (int i = 0; i < playerTransforms.Count; i++)
        {
            PlayerStats newPlayerStats = new PlayerStats(playerTransforms[i], playerTransforms[i].GetComponent<PlayerHandler>());

            players.Add(newPlayerStats);
        }
    }

    

    private void Start()
    {
        GetComponent<HandleStart>().HandleTheStart(); 
                 
    }

    public async void StartNewTurn()
    {
        state = BoardStates.TurnOfAPlayer;
        blackScreen.GetComponent<Animator>().SetTrigger("PlayAnimation");
        await Task.Delay(300);
        _playerIndex++;
        players[_playerIndex].handler.StartTurn();
    }
}
