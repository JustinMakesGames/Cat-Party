using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleDicePlayerAction : MonoBehaviour
{
    public DoubleDiceScript diceScript;
    public bool canHitDice;

    public void AssignItem(DoubleDiceScript diceScript)
    {
        this.diceScript = diceScript;
    }

    public void PlayerHitDiceBlock(InputAction.CallbackContext context)
    {
        if (context.performed && canHitDice)
        {
            canHitDice = false;
            diceScript.HitDiceBlock();
        }
    }
}
