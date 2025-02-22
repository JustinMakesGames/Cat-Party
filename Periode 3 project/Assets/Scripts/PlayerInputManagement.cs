using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[Serializable]
public struct InputID
{
    public InputDevice inputDevice;
    public int id;

    public InputID(InputDevice inputDevice, int id) 
    {
        this.inputDevice = inputDevice;
        this.id = id;
    }
}
public class PlayerInputManagement : MonoBehaviour
{
    public static PlayerInputManagement Instance;
    [SerializeField] private List<GameObject> menuPlayers;
    [SerializeField] private Transform playerFolder;
    [SerializeField] private Transform spawnFolder;
    private int playerAmount = 0;
    public List<InputID> inputDevices = new List<InputID>();

    private bool _canConnectNewPlayers;

    private void Awake()
    {
        _canConnectNewPlayers = true;
        if (Instance == null) 
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(playerFolder.gameObject);

        
    }

    private void Start()
    {
        SceneManager.sceneLoaded += DisableConnectingPlayers;
    }
    private void DisableConnectingPlayers(Scene scene, LoadSceneMode loadMode) 
    {
        _canConnectNewPlayers = false;
    }

    public void EnableConnectingPlayers() 
    {
        _canConnectNewPlayers = true;
    }
    public void HandleSceneSwitch() 
    {
        playerFolder = GameObject.FindGameObjectWithTag("PlayerFolder").transform;
    }
    

    


    public void PlayerJoins(PlayerInput playerInput) 
    {
        if (SceneManager.GetActiveScene().name == "Menu") 
        {
            MenuJoin(playerInput);
        }

        else if (SceneManager.GetActiveScene().name == "BoardGame") 
        {
            BoardGameJoin(playerInput);
        } 
        
        
    }

    private void MenuJoin(PlayerInput playerInput) 
    {
        if (!playerInput.transform.CompareTag("Player")) 
        {
            inputDevices.Add(ReturnInputDevice(playerInput.devices[0]));
            Instantiate(menuPlayers[GetRightID()], spawnFolder.GetChild(GetRightID()).position, Quaternion.identity, playerFolder);       
            Destroy(playerInput.gameObject);
        }

        else 
        {
            
            playerAmount++;

        }
    }
    private void BoardGameJoin(PlayerInput playerInput) 
    {
        if (!_canConnectNewPlayers) return;
        if (playerInput.transform.parent != playerFolder) 
        {
            print("This code was played");
            Destroy(playerInput.gameObject);
            playerFolder.GetChild(playerAmount).GetComponent<PlayerInput>().enabled = true;         
        }

        else 
        {
            
            inputDevices.Add(ReturnInputDevice(playerInput.devices[0]));
            playerAmount++;
        }
    }

    public void PlayerLeaves(PlayerInput playerInput) 
    {    
        StartCoroutine(DisablePlayerInput(playerInput));

    }

    private IEnumerator DisablePlayerInput(PlayerInput playerInput) 
    {
        yield return null;
        if (playerInput == null || !playerInput.transform.CompareTag("Player")) yield break;
        playerAmount--;
        print(playerInput.transform.name + " leaves the game");

        print("valid: " + playerInput.user.valid);
        playerInput.enabled = false;

    }

    private InputID ReturnInputDevice(InputDevice inputDevice) 
    {
        int rightID = 0;
        for (int i = 0; i < inputDevices.Count; i++) 
        {
            if (inputDevices[i].id != i) 
            {
                rightID = i;
                break;
            }

            else 
            {
                rightID = inputDevices.Count;
            }
        }
        InputID newInput = new InputID(inputDevice, rightID);

        print("The id is: " + newInput.id);
        return newInput;
    }

    private int GetRightID() 
    {
        return inputDevices[inputDevices.Count - 1].id;
    }
     
}
