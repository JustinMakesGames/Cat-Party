using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class BackToMenu : MonoBehaviour
{
    [SerializeField] private Transform playerFolder;
    public void RemovePlayers()
    {
        foreach (Transform player in playerFolder)
        {
            player.GetComponent<PlayerLeavesGame>().DeviceLost(player.GetComponent<PlayerInput>());
        }
    }
}
