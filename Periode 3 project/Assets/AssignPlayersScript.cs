using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssignPlayersScript : MonoBehaviour
{
    public List<PlayerInput> rightPlayerInput;
    private void Start()
    {
        for (int i = 0; i < PlayerInputManagement.Instance.inputDevices.Count; i++) 
        {
            rightPlayerInput[i].enabled = true;
            rightPlayerInput[i].SwitchCurrentControlScheme(PlayerInputManagement.Instance.inputDevices[i]);
        }
    }

}
