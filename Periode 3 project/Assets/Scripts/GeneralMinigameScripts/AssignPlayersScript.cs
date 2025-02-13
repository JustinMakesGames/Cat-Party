using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssignPlayersScript : MonoBehaviour
{
    public List<PlayerInput> rightPlayerInput;
    private void Start()
    {
        PlayerInputManagement.Instance.HandleSceneSwitch();
        for (int i = 0; i < PlayerInputManagement.Instance.inputDevices.Count; i++) 
        {
            PlayerInput playerInput = rightPlayerInput[PlayerInputManagement.Instance.inputDevices[i].id];

            playerInput.enabled = true;
            if (playerInput.user.valid)
            {
                playerInput.user.UnpairDevices();
            }
            playerInput.SwitchCurrentControlScheme(PlayerInputManagement.Instance.inputDevices[i].inputDevice);
            
        }
        PlayerInputManagement.Instance.EnableConnectingPlayers();
    }

}
