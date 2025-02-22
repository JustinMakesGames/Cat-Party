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
    public int yarnAmount;
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

    public bool canClickThroughText;

    private void Awake()
    {
        _cam = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
        pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform;
        _eventSystem = GetComponentInChildren<MultiplayerEventSystem>();


    }
    public void ClickThroughText(InputAction.CallbackContext context)
    {
        if (context.performed && canClickThroughText)
        {
            TextListScript.Instance.SetButtonTrue();
            canClickThroughText = false;
        }
    }

    public void CanClickThroughText()
    {
        canClickThroughText = true;
    }
    private void Start()
    {
        SceneManager.sceneLoaded += AssignPlayerHandlerToMinigame;
    }

    public IEnumerator StartTurn()
    {
        _cam.position = transform.GetChild(1).position;
        _cam.parent = transform;
        if (GetComponent<PlayerInput>().enabled)
        {
            isPlayer = true;
        }

        yield return new WaitForSeconds(1);
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
        StartCoroutine(HandleOutCome());

    }

    private IEnumerator HandleOutCome()
    {

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(pathFolder.GetChild(currentSpace).GetComponent<SpaceHandler>().HandleLandedPlayer(transform));

        StartCoroutine(BoardGameManager.Instance.StartNewTurn());
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

    public void ChangeYarnValue(int yarnValue)
    {
        yarnAmount += yarnValue;

        if (yarnAmount < 0) yarnAmount = 0;

        yarnText.text = "x" + yarnAmount.ToString();
    }

    private void AssignPlayerHandlerToMinigame(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MinigameTest")
        {
            print("Played in " + transform.name);
            Transform minigamePlayerFolder = GameObject.FindGameObjectWithTag("MinigamePlayerFolder").transform;

            minigamePlayerFolder.GetChild(transform.GetSiblingIndex()).GetComponent<MinigamePlayerHandler>().playerHandler = this;
        }

        if (SceneManager.GetActiveScene().name == "BoardGame")
        {
            _cam = Camera.main.transform;
            pathFolder = GameObject.FindGameObjectWithTag("PathFolder").transform;
        }
    }
}
