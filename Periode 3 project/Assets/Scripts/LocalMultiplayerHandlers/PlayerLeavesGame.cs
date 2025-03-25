using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLeavesGame : MonoBehaviour
{
    public int index;
    private bool _wasEnabled;

    private void Update()
    {
        if (GetComponent<PlayerInput>().enabled && !_wasEnabled)
        {
            PlayerInputManagement.Instance.AddPlayerToList(GetComponent<PlayerInput>().devices[0], index);
            _wasEnabled = true;
        }

        else if (!GetComponent<PlayerInput>().enabled && _wasEnabled)
        {
            _wasEnabled = false;
        }
    }
    public void DeviceLost(PlayerInput playerInput) 
    {
        PlayerInputManagement.Instance.RemovePlayerOfList(index);
        if (transform.CompareTag("MenuPlayer"))
        {
            Destroy(gameObject);
        }

        else
        {
            PlayerInputManagement.Instance.PlayerLeaves(playerInput);
        }
        

    }

    public void Regained(PlayerInput playerInput) 
    {
        print("This is the same controller as before.");

    }
}
