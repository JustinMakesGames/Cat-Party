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

            rightPlayerInput[PlayerInputManagement.Instance.inputDevices[i].id].enabled = true;
            rightPlayerInput[PlayerInputManagement.Instance.inputDevices[i].id].
                SwitchCurrentControlScheme(PlayerInputManagement.Instance.inputDevices[i].inputDevice);
            
        }
        PlayerInputManagement.Instance.EnableConnectingPlayers();
    }

}
