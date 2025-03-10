using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPotatoManager : MonoBehaviour, IMinigameManager
{
    public static HotPotatoManager Instance;
    public bool hasMinigameStarted;
    [SerializeField] private GameObject bomb;
    [SerializeField] private List<Transform> players;
    private GameObject _bombClone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void BeginMinigame()
    {
        foreach (Transform t in players)
        {
            t.GetComponent<MinigamePlayerMovement>().StartMinigame();
        }

        StartCoroutine(CheckIfBombExists());
    }

    private IEnumerator CheckIfBombExists()
    {
        while (true)
        {
            if (_bombClone == null)
            {
                SpawnBomb();
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void SpawnBomb()
    {
        int randomPlayer = Random.Range(0, players.Count);

        _bombClone = Instantiate(bomb);

        players[randomPlayer].GetComponent<HotPotatoMovement>().GetBombed(_bombClone.transform);
    }

    public void KillPlayer(Transform player)
    {
        players.Remove(player);

        if (players.Count == 1)
        {
            StopAllCoroutines();
            MinigameManager.Instance.EndMinigame(players[0].GetComponent<MinigamePlayerHandler>().playerHandler);
        }
    }

    
}
