using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarsManager : MonoBehaviour, IMinigameManager
{
    
    [SerializeField] private Transform playerFolder;
    private List<PlayerHandler> playerHandlers = new List<PlayerHandler>();
    public void BeginMinigame()
    {
        AssignPlayers();
        MinigameManager.Instance.EndMinigame(playerHandlers[0]);
    }

    private void AssignPlayers()
    {
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            playerHandlers.Add(playerFolder.GetChild(i).GetComponent<MinigamePlayerHandler>().playerHandler);
        }
    }
}
