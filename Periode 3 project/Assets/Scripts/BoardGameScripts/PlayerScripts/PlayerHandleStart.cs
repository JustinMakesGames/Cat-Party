using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandleStart : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator modelAnimator;

    private bool _canRollDice;
    private bool _isPlayer;

    public int rolledNumber;
   

    

    public void FirstDiceRollPlayer(InputAction.CallbackContext context)
    {
        if (context.performed && _canRollDice)
        {
            AudioHandling.Instance.DiceRoll();
            animator.SetTrigger("Jump");
            modelAnimator.SetTrigger("Jump");
            HandleStart.Instance.RollDice(transform);
            _canRollDice = false;
        }
    }

    private IEnumerator FirstDiceRollCPU()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2.5f));
        AudioHandling.Instance.DiceRoll();
        animator.SetTrigger("Jump");
        HandleStart.Instance.RollDice(transform);
        _canRollDice = false;
    }

    public void SetRollDice()
    {
        if (GetComponent<PlayerInput>().enabled)
        {
            _isPlayer = true;
        }
        _canRollDice = true;

        if (!_isPlayer)
        {
            StartCoroutine(FirstDiceRollCPU());
        }
        
    }

    public void SetNumber(int number)
    {
        rolledNumber = number;
    }
}
