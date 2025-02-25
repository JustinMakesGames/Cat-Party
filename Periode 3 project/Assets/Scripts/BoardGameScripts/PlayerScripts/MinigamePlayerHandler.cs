using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamePlayerHandler : MonoBehaviour
{
    public PlayerHandler playerHandler;
    public bool isStartingMinigame;

    public void SetPlayerHandler(PlayerHandler handler)
    {
        playerHandler = handler;
    }
    public void SetMinigamePlayerOn()
    {
        print("Sets it on in " + transform.name);
        isStartingMinigame = true;
    }
    public void StartMinigame(InputAction.CallbackContext context)
    {
        if (context.performed &&  isStartingMinigame)
        {
            isStartingMinigame = false;
            StartCoroutine(MinigameManager.Instance.StartMinigame());

        }
    }
}
