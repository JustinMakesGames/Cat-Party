using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.UI;

public class PlayerHandler : MonoBehaviour
{
    public Transform pathFolder;
    public int coinAmount;
    public int woolAmount;
    public int currentSpace;
    public bool isPlayer;
    public Color color;
    public TMP_Text yarnText, coinText;
    [SerializeField] private GameObject chooseCanvas;
    [SerializeField] private GameObject introScreen;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject outcomeCanvas;
    

    
    private GameObject _outcomeCanvasClone;
    private GameObject _diceClone;
    private Transform _cam;
    private Animator _animator;
    private MultiplayerEventSystem _eventSystem;

    private bool _canHitDice;
    private void Awake()
    {
        _cam = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
        pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform;
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public async void StartTurn()
    {
        _cam.position = transform.GetChild(1).position;
        _cam.parent = transform;
        if (GetComponent<PlayerInput>().enabled)
        {
            isPlayer = true;
        }

        await Task.Delay(1000);
        introScreen.SetActive(true);
    }

    public void CheckIfPlayerOrCPU()
    {
        if (isPlayer) PlayerTurn();
        else CPUTurn();
    }

    private void PlayerTurn()
    {
        chooseCanvas.SetActive(true);
        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(chooseCanvas.transform.GetChild(0).GetChild(0).gameObject);
    }

    public void SpawnDicePlayer()
    {
        _diceClone = Instantiate(dice, transform.GetChild(0).position, Quaternion.identity, transform);
        _canHitDice = true;
    }
    private void CPUTurn()
    {
        _diceClone = Instantiate(dice, transform.GetChild(0).position, Quaternion.identity, transform);
        StartCoroutine(HandleCPUDice());
    }

    private IEnumerator HandleCPUDice()
    {
        yield return new WaitForSeconds(1);
        HitDiceBlock();

    }

    private async void HitDiceBlock()
    {
        
        _animator.SetTrigger("Jump");
        int randomValue = Random.Range(1, 7);
        Destroy(_diceClone);
        _outcomeCanvasClone = Instantiate(outcomeCanvas, transform.GetChild(0).position, Quaternion.identity, transform);
        _outcomeCanvasClone.GetComponentInChildren<TMP_Text>().text = randomValue.ToString();

        await Task.Delay(2000);
        currentSpace = await GetComponent<HandleWalking>().StartHandlingWalking(randomValue, currentSpace, _outcomeCanvasClone.GetComponentInChildren<TMP_Text>());
        HandleOutCome();

    }

    private async void HandleOutCome()
    {

        await Task.Delay(1000);

        await pathFolder.GetChild(currentSpace).GetComponent<SpaceHandler>().HandleLandedPlayer(transform);

        BoardGameManager.Instance.StartNewTurn();
    }
    public void PlayerHitDiceBlock(InputAction.CallbackContext context)
    {
        if (context.performed && _canHitDice)
        {
            _canHitDice = false;
            HitDiceBlock();
        }
    }

    public void GoBack(InputAction.CallbackContext context)
    {
        if (context.performed && _canHitDice)
        {
            Destroy(_diceClone);
            _canHitDice = false;
            CheckIfPlayerOrCPU();
        }
    }

    public void ChangeCoinValue (int coinValue)
    {
        coinAmount += coinValue;  
        
        if (coinAmount < 0) coinAmount = 0;
        coinText.text = "x" + coinAmount.ToString();

        
    }
    


    
}
