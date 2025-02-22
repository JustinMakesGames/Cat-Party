using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamePlayerHandler : MonoBehaviour
{
    public PlayerHandler playerHandler;
    private bool _isStartingMinigame;

    public void SetPlayerHandler(PlayerHandler handler)
    {
        playerHandler = handler;
    }
    public void SetMinigamePlayerOn()
    {
        print("Sets it on in " + transform.name);
        _isStartingMinigame = true;
    }
    public void StartMinigame(InputAction.CallbackContext context)
    {
        if (context.performed &&  _isStartingMinigame)
        {
            _isStartingMinigame = false;
            StartCoroutine(MinigameManager.Instance.StartMinigame());

        }
    }
}
