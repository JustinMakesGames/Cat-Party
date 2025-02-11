using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManagement : MonoBehaviour
{
    public static PlayerInputManagement Instance;
    [SerializeField] private Transform playerFolder;
    private int playerAmount = 0;

    public List<PlayerInput> playerInputs;
    public List<InputDevice> inputDevices = new List<InputDevice>();

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(playerFolder.gameObject);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            SceneManager.LoadScene("Marijn");
        }
    }


    public void PlayerJoins(PlayerInput playerInput) 
    {
        if (SceneManager.GetActiveScene().name != "Justin") return;
        if (playerInput.transform.parent != playerFolder) 
        {
            Destroy(playerInput.gameObject);
            playerFolder.GetChild(playerAmount).GetComponent<PlayerInput>().enabled = true;
            
            playerAmount++;
            
        }

        else 
        {
            playerInputs.Add(playerInput);
            inputDevices.Add(playerInput.devices[0]);
        }
        
    }

    public void PlayerLeaves(PlayerInput playerInput) 
    {
        if (playerInput.transform.parent != playerFolder) return;
        playerAmount--;
        print(playerInput.transform.name + " leaves the game");

        print("valid: " + playerInput.user.valid);
        playerInputs.Remove(playerInput);
        DisablePlayerInput(playerInput);

    }

    private IEnumerator DisablePlayerInput(PlayerInput playerInput) 
    {
        yield return null;
        playerInput.enabled = false;

    }
     
}
