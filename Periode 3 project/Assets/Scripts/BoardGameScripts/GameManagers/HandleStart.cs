using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HandleStart : MonoBehaviour
{
    public static HandleStart Instance;
    [SerializeField] private List<string> textPrompts;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject numberOutcome;
    private bool _hasPressedButton;
    private List<Transform> _players = new List<Transform>();
    private List<int> _allPossibleNumbers = new List<int>() 
    {
        1, 2, 3, 4, 5, 6
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
        ShowText();
    }

    private void AssignPlayers()
    {
        _players = BoardGameManager.Instance.playerTransforms;
    }

    private async void ShowText()
    {
        await TextListScript.Instance.ShowPrompts(TextListScript.Instance.textStrings[0].strings);
        HandleDiceRoll();
    }

    private async void TellPlayerOrder()
    {
        await Task.Delay(1400);

        foreach (var player in _players)
        {
            Destroy(player.GetChild(0).gameObject);
        }

        await Task.Delay(100);
        Vector3 originalCameraPosition = Camera.main.transform.position;
        List<string> numbers = new List<string>() {
            "first", "second", "third", "fourth"
        };

        for (int i = 0; i < _players.Count; i++)
        {
            string prompt = _players[i].name + " goes " + numbers[i] + ".";
            Camera.main.transform.position = _players[i].GetChild(1).position;
            await TextListScript.Instance.ShowPrompt(prompt);
        }

        Camera.main.transform.position = originalCameraPosition;

        await TextListScript.Instance.ShowPrompts(TextListScript.Instance.textStrings[1].strings);

        BoardGameManager.Instance.StartNewTurn();
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
            TellPlayerOrder();
        }
    }

    private int ReturnNumber()
    {
        int randomNumber = _allPossibleNumbers[Random.Range(0, _allPossibleNumbers.Count)];

        _allPossibleNumbers.Remove(randomNumber);

        return randomNumber;
    }
}
