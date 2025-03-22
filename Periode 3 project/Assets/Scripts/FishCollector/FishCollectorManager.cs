using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public struct FishCollectorPlayerPoints
{
    public PlayerHandler playerHandler;
    public int points;

    public FishCollectorPlayerPoints(PlayerHandler playerHandler, int points)
    {
        this.playerHandler = playerHandler;
        this.points = points;
    }
}
public class FishCollectorManager : MonoBehaviour, IMinigameManager
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private int timer;
    [SerializeField] private List<Transform> players = new List<Transform>();
    [SerializeField] private GameObject fish, redFish, spikeBall;
    [SerializeField] private Collider spawnPlace;
    [SerializeField] private int minSpawnRedFish;
    [SerializeField] private int maxSpawnRedFish;
    [SerializeField] private int minSpawnSpikeBall;
    [SerializeField] private int maxSpawnSpikeBall;

    [SerializeField] private float timeInBetween;
    [SerializeField] private float countDownTimeInBetween;

    [SerializeField] private Transform fishFolder;
    private int _spawnRedFishChance;
    private int _chosenRedFishSpawn;
    private int _spawnSpikeBallChance;
    private int _chosenSpikeBallSpawn;

    private Bounds spawnPlaceBounds;

    public void BeginMinigame()
    {
        spawnPlaceBounds = spawnPlace.bounds;
        foreach (Transform t in players)
        {
            t.GetComponent<MinigamePlayerMovement>().StartMinigame();
        }

        StartCoroutine(CountTimerDown());
        StartCoroutine(GameLoop());
        StartCoroutine(CountDownCoroutine());

        
    }

    private IEnumerator CountTimerDown()
    {
        timerText.text = timer.ToString();
        while (true)
        {
            yield return new WaitForSeconds(1);
            timer--;
            timerText.text = timer.ToString();
            if (timer <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(EndGame());
            }
        }
    }

    
    private IEnumerator GameLoop()
    {
        MakeNewRedFishChance();
        MakeNewSpikeBallChance();
        while (true)
        {
            

            for (int i = 0; i < 3; i++)
            {
                if (_spawnRedFishChance >= _chosenRedFishSpawn)
                {
                    SpawnFish(redFish);
                    MakeNewRedFishChance();

                }

                else if (_spawnSpikeBallChance >= _chosenSpikeBallSpawn)
                {
                    SpawnFish(spikeBall);
                    MakeNewSpikeBallChance();
                }

                else
                {
                    SpawnFish(fish);
                }
            }

            

            CountChancesUp();
            yield return new WaitForSeconds(timeInBetween);
        }
       
    }

    private IEnumerator CountDownCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            CountDown();
        }
    }

    private void MakeNewRedFishChance()
    {
        _chosenRedFishSpawn = Random.Range(minSpawnRedFish, maxSpawnRedFish);
        _spawnRedFishChance = 0;

    }

    private void MakeNewSpikeBallChance()
    {
        _chosenSpikeBallSpawn = Random.Range(minSpawnSpikeBall, maxSpawnSpikeBall);
        _spawnSpikeBallChance = 0;

    }

    private void CountChancesUp()
    {
        _spawnRedFishChance++;
        _spawnSpikeBallChance++;
    }
    private void SpawnFish(GameObject obj)
    {
        
        Vector3 positionToSpawn = new Vector3(Random.Range(spawnPlaceBounds.min.x, spawnPlaceBounds.max.x), spawnPlaceBounds.max.y, 
            Random.Range(spawnPlaceBounds.min.z, spawnPlaceBounds.max.z));

        Instantiate(obj, positionToSpawn, Quaternion.identity, fishFolder);
    }

    private void CountDown()
    {
        timeInBetween -= countDownTimeInBetween;
        countDownTimeInBetween *= 0.99f;
    }

    private IEnumerator EndGame()
    {

        DestroyAllFish();
        EndMinigameForPlayers();
        yield return new WaitForSeconds(1);
        ShowAllPoints();
        CalculateOrder();
        yield return new WaitForSeconds(2);
        MinigameManager.Instance.EndMinigame();

    }

    private void DestroyAllFish()
    {
        foreach (Transform f in fishFolder)
        {
            Destroy(f.gameObject);
        }
    }

    private void EndMinigameForPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<FishCollectorMovement>().MinigameEnded();
        }
    }
    private void ShowAllPoints()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetChild(1).gameObject.SetActive(true);
            players[i].GetComponentInChildren<TMP_Text>().text = players[i].GetComponent<FishCollectorMovement>().points.ToString();
        }
    }

    private void CalculateOrder()
    {
        List<FishCollectorPlayerPoints> playerPointList = new List<FishCollectorPlayerPoints>();


        for (int i = 0; i < players.Count; i++)
        {
            FishCollectorPlayerPoints playerPoints = new FishCollectorPlayerPoints(players[i].GetComponent<MinigamePlayerHandler>().playerHandler,
                players[i].GetComponent<FishCollectorMovement>().points);
            playerPointList.Add(playerPoints);
        }

        playerPointList.Sort((a, b) => a.points.CompareTo(b.points));

        for (int i = 0; i < 3; i++)
        {
            MinigameManager.Instance.ThrowPlayerInDictionary(playerPointList[i].playerHandler, i);
        }
    }
}
