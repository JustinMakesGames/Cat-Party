using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleDiceScript : Item
{
    
    [SerializeField] private int maxHittingDice;
    [SerializeField] private GameObject dice;
    [SerializeField] private GameObject outcomeCanvas;
    [SerializeField] private float canvasSpeed;
    

    private GameObject _diceClone;
    private List<int> rolledNumbers = new List<int>();
    private List<GameObject> _outcomeCanvasClones = new List<GameObject>();
    private int _diceHitAmount;
    private Animator _animator;
    private GameObject _outcomeCanvasClone;
    private int _finalRolledNumber;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override IEnumerator BeginUsingItem()
    {
        
        player.GetComponent<DoubleDicePlayerAction>().AssignItem(this);
        _animator = player.GetComponentInChildren<Animator>();
        SpawnDoubleDice();
        yield return null;
    }

    
    private void SpawnDoubleDice()
    {
        _diceClone = Instantiate(dice, player.GetChild(0).position, Quaternion.identity);

        if (player.GetComponent<PlayerHandler>().isPlayer)
        {
            player.GetComponent<DoubleDicePlayerAction>().canHitDice = true;
        }

        else
        {
            StartCoroutine(CPUAction());
        }
        
    }
    
    private IEnumerator CPUAction()
    {
        yield return new WaitForSeconds(1);
        HitDiceBlock();
    }
    public void HitDiceBlock()
    {
        _animator.SetTrigger("Jump");
        int randomValue = Random.Range(1, 11);
        Destroy(_diceClone);
        _outcomeCanvasClone = Instantiate(outcomeCanvas, player.GetChild(0).position, Quaternion.identity, player);
        _outcomeCanvasClone.GetComponentInChildren<TMP_Text>().text = randomValue.ToString();

        _diceHitAmount++;
        rolledNumbers.Add(randomValue);
        DecidePositionOfCanvas();

        StartCoroutine(DecideIfHitAgain());        
    }

    private IEnumerator DecideIfHitAgain()
    {
        yield return new WaitForSeconds(1);

        if (_diceHitAmount < maxHittingDice)
        {
            SpawnDoubleDice();
        }

        else
        {
            StartCoroutine(MoveTowardsEachOther());
        }
    }

    private void DecidePositionOfCanvas()
    {
        switch (_diceHitAmount)
        {
            case 1:
                CalculatePositionOfCanvas(-4);
                break;
            case 2:
                CalculatePositionOfCanvas(4);
                break;
            case 3:
                CalculatePositionOfCanvas(0);
                break;
        }
    }

    private IEnumerator MoveTowardsEachOther()
    {
        float timer = 0;
        float maxTimer = 1;

        while (timer < maxTimer)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < _outcomeCanvasClones.Count; i++)
            {
                _outcomeCanvasClones[i].transform.position = Vector3.MoveTowards(_outcomeCanvasClones[i].transform.position, player.GetChild(0).position,
                    canvasSpeed * Time.deltaTime);
            }

            yield return null;
        }
        
        CalculateNumber();
        
    }

    private void CalculateNumber()
    {
        for (int i = 0; i < rolledNumbers.Count; i++)
        {
            _finalRolledNumber += rolledNumbers[i];

        }

        for (int i = 0; i < _outcomeCanvasClones.Count; i++)
        {
            Destroy(_outcomeCanvasClones[i]);
        }

        _outcomeCanvasClone = Instantiate(outcomeCanvas, player.GetChild(0).position, Quaternion.identity, player);
        _outcomeCanvasClone.GetComponentInChildren<TMP_Text>().text = _finalRolledNumber.ToString();

        player.GetComponent<PlayerHandler>().CalculateMovement(_finalRolledNumber, _outcomeCanvasClone);
    }
    private void CalculatePositionOfCanvas(int xPosition)
    {
        float finalXPosition = _outcomeCanvasClone.transform.position.x + xPosition;
        _outcomeCanvasClone.transform.position = new Vector3(finalXPosition, _outcomeCanvasClone.transform.position.y, _outcomeCanvasClone.transform.position.z);
        _outcomeCanvasClones.Add(_outcomeCanvasClone);
    }


}
