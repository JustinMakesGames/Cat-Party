using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamePrint : MonoBehaviour
{

    public void HasJumped(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            print("Wow this player is a high jumper");
        }
        
    }
}
