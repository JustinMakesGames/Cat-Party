using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeavesGame : MonoBehaviour
{
    public void DeviceLost(PlayerInput playerInput) 
    {
        PlayerInputManagement.Instance.PlayerLeaves(playerInput);

    }

    public void Regained(PlayerInput playerInput) 
    {
        print("This is the same controller as before.");

    }
}
