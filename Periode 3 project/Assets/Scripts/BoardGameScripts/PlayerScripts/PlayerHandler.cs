using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    public bool isPlayer;
    private PlayerInput player;

    private void Awake()
    {
        player = GetComponent<PlayerInput>();
    }
    public void OnSelected(InputAction.CallbackContext context)
    {
        if (context.performed) 
        {
            print("The player index is " + player.playerIndex);
            print("The player object is " + gameObject);
        }
        
        
    }
    


    
}
