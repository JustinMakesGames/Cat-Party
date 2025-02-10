using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerFolder;
    
    private List<PlayerHandler> players;
    
    private void Awake()
    {
        foreach (PlayerHandler player in players)
        {
            players.Add(player);
        }
    }
}
