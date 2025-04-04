using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandleStart : MonoBehaviour
{
    public static HandleStart Instance;
    [SerializeField] private List<string> textPrompts;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject numberOutcome;
    [SerializeField] private Transform playerUIFolder;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject coinCollectCanvas;
    private bool _hasPressedButton;
    private List<Transform> _players = new List<Transform>();
    private List<int> _allPossibleNumbers = new List<int>() 
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    };
    private int rolledDicesAmount;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void HandleTheStart()
    {
        AssignPlayers();
        StartCoroutine(ShowText());
    }

    private void AssignPlayers()
    {
        _players = BoardGameManager.Instance.playerTransforms;
    }

    private IEnumerator ShowText()
    {
        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(TextListScript.Instance.textStrings[0].strings));
        HandleDiceRoll();
    }

    private IEnumerator TellPlayerOrder()
    {
        yield return new WaitForSeconds(1.4f);

        foreach (var player in _players)
        {
            Destroy(player.GetChild(0).gameObject);
        }

        yield return new WaitForSeconds(0.1f);
        Vector3 originalCameraPosition = Camera.main.transform.position;
        List<string> numbers = new List<string>() {
            "first", "second", "third", "fourth"
        };

        for (int i = 0; i < _players.Count; i++)
        {
            string prompt = _players[i].name + " goes " + numbers[i] + ".";
            Camera.main.transform.position = _players[i].GetChild(1).position;
            yield return StartCoroutine(TextListScript.Instance.ShowPrompt(prompt));
        }

        Camera.main.transform.position = originalCameraPosition;

        yield return StartCoroutine(TextListScript.Instance.ShowPrompts(TextListScript.Instance.textStrings[1].strings));
        yield return StartCoroutine(GivePlayersCoins());

        yield return StartCoroutine(YarnPlacement.Instance.StartShowingYarnPlacement());

        StartCoroutine(BoardGameManager.Instance.StartNewTurn());
    }

    private IEnumerator GivePlayersCoins()
    {
        string givePlayersCoins = "You will all start with 10 coins each.";

        yield return StartCoroutine(TextListScript.Instance.ShowPrompt(givePlayersCoins));

        foreach (Transform player in _players)
        {
            StartCoroutine(CoinChange.Instance.WinCoins(player, 10));
        }

        yield return new WaitForSeconds(2);
    }

    
    private void HandleDiceRoll()
    {
        foreach (Transform p in _players)
        {
            GameObject diceClone = Instantiate(dice, p.GetChild(0).position, Quaternion.identity, p);
            diceClone.transform.SetAsFirstSibling();
            p.GetComponent<PlayerHandleStart>().SetRollDice();
            
        }
    }

    public void RollDice(Transform player)
    {
        int rolledNumber = ReturnNumber();
        GameObject canvas = Instantiate(numberOutcome, player.GetChild(0).position, Quaternion.identity, player);
        
        canvas.GetComponentInChildren<TMP_Text>().text = rolledNumber.ToString();
        Destroy(player.GetChild(0).gameObject);
        canvas.transform.SetAsFirstSibling();
        player.GetComponent<PlayerHandleStart>().SetNumber(rolledNumber);

        CheckIfAllPlayersRolled();


        
    }

    private void CheckIfAllPlayersRolled()
    {
        rolledDicesAmount++;

        if (rolledDicesAmount == 4)
        {
            BoardGameManager.Instance.MakeTheTurnOrder();
            _players = BoardGameManager.Instance.playerTransforms;
            AssignPlayerOrderUI();
            StartCoroutine(TellPlayerOrder());
        }
    }

    private void AssignPlayerOrderUI()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            Image image = playerUIFolder.GetChild(i).GetComponent<Image>();
            image.gameObject.SetActive(true);
            Color color = _players[i].GetComponent<PlayerHandler>().color;
            image.color = color;

            image.transform.GetChild(image.transform.childCount - 1).GetComponent<Image>().sprite = _players[i].GetComponent<PlayerHandler>().image;
            _players[i].GetComponent<PlayerHandler>().yarnText = image.transform.GetChild(0).GetComponent<TMP_Text>();
            _players[i].GetComponent<PlayerHandler>().coinText = image.transform.GetChild(1).GetComponent<TMP_Text>();
            
        }
    }

    private int ReturnNumber()
    {
        int randomNumber = _allPossibleNumbers[Random.Range(0, _allPossibleNumbers.Count)];

        _allPossibleNumbers.Remove(randomNumber);

        return randomNumber;
    }
}
