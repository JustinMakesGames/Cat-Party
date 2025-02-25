using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        get => handler.yarnAmount;
        set => handler.yarnAmount = value;
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
    [SerializeField] private GameObject canvasObject;
    
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

        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(canvasObject);
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
        print("Played the start of the boardgamemanager");
        GetComponent<HandleStart>().HandleTheStart(); 
                 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("MinigameTest");
        }
    }

    public IEnumerator StartNewTurn()
    {
        state = BoardStates.TurnOfAPlayer;
        blackScreen.GetComponent<Animator>().SetTrigger("FadeInOut");

        Camera.main.transform.parent = null;
        SceneManager.MoveGameObjectToScene(Camera.main.gameObject, SceneManager.GetActiveScene());

        yield return new WaitForSeconds(0.3f);

        if (_playerIndex == 3)
        {
            _playerIndex = -1;
            StartCoroutine(MinigameManager.Instance.HandleMinigameTime());
        }

        else
        {
            _playerIndex++;
            StartCoroutine(players[_playerIndex].handler.StartTurn());
        }
        
        
    }

    public void HandleReturnToBoardGame(PlayerHandler winnerPlayerHandler)
    {
        BoardPlacementManager.Instance.ShowBoardPlacement(winnerPlayerHandler);
    }

    
}
