using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
    [SerializeField] private TMP_Text roundText, maxRoundText;
    [SerializeField] private float animationDuration;
    [SerializeField] private int maxTurnAmount;
    [SerializeField] private Transform camPosition;
    
    public List<Transform> playerTransforms = new List<Transform>();
    public List<PlayerStats> players = new List<PlayerStats>();

    private int _playerIndex = -1;
    private bool _gameHasStarted;


    private int _turnAmount = 1;
    private bool _isDebugging;

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

    public void DebuggingOn()
    {
        _isDebugging = true;
    }

    private void Start()
    {
        if (_isDebugging) return;
        GetComponent<HandleStart>().HandleTheStart(); 
                 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("PlatformChaos");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("HotPotato");
        }


        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            SceneManager.LoadScene("FishCollector");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("ReactingKittens");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Sweeper");
        }
    }

    public IEnumerator StartNewTurn()
    {
        Camera.main.transform.parent = null;
        SceneManager.MoveGameObjectToScene(Camera.main.gameObject, SceneManager.GetActiveScene());

        if (_turnAmount >= maxTurnAmount)
        {
            StartCoroutine(GetComponent<WinManager>().EndGame());
            yield break;
        } 
        if (_playerIndex == -1)
        {
            
            blackScreen.GetComponent<Animator>().SetTrigger("FadeInOut");
            yield return new WaitForSeconds(0.2f);
            Camera.main.transform.position = camPosition.position;
            yield return StartCoroutine(ShowNextTurn());
        }
        
        state = BoardStates.TurnOfAPlayer;
        blackScreen.GetComponent<Animator>().SetTrigger("FadeInOut");

        

        yield return new WaitForSeconds(0.3f);

        if (_playerIndex == 3)
        {
            _turnAmount++;
            _playerIndex = -1;
            StartCoroutine(MinigameManager.Instance.HandleMinigameTime());
        }

        else
        {
            _playerIndex++;
            StartCoroutine(players[_playerIndex].handler.StartTurn());
        }
        
        
    }

    private IEnumerator ShowNextTurn()
    {
        roundText.gameObject.SetActive(true);
        maxRoundText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        UpdateNumber();
        yield return new WaitForSeconds(2);
        roundText.gameObject.SetActive(false);
        maxRoundText.gameObject.SetActive(false);
    }

    private void UpdateNumber()
    {
        int oldTurnAmount = _turnAmount - 1;
        TMP_Text oldText = Instantiate(roundText, roundText.transform.parent);
        oldText.text = oldTurnAmount.ToString();

        oldText.rectTransform.DOAnchorPosY(roundText.rectTransform.anchoredPosition.y - 50, animationDuration);
        oldText.DOFade(0, animationDuration).OnComplete(() => Destroy(oldText.gameObject));
        roundText.text = _turnAmount.ToString();
        roundText.rectTransform.anchoredPosition += new Vector2(0, 50);
        roundText.alpha = 0;

        roundText.DOFade(1, animationDuration);
        roundText.rectTransform.DOAnchorPosY(roundText.rectTransform.anchoredPosition.y - 50, animationDuration);
    }

    public void HandleReturnToBoardGame(Dictionary<PlayerHandler, int> winnerPlayerHandler)
    {
        BoardPlacementManager.Instance.ShowBoardPlacement(winnerPlayerHandler);
    }

    
}
