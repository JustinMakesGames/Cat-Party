using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ReactCatsManager : MonoBehaviour, IMinigameManager
{
    public static ReactCatsManager Instance;
    [SerializeField] private List<Transform> players;
    [SerializeField] private GameObject fish, badFish;
    [SerializeField] private int totalFishAmount;
    [SerializeField] private Transform spawnPlace;
    [SerializeField] private Transform middlePlace;
    [SerializeField] private Transform endPlace;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float timeToReact;
    [SerializeField] private int goodFishAmount;
    [SerializeField] private int badFishAmount;

    [SerializeField] private List<int> pointAmounts = new List<int>();

    private List<GameObject> fishOrder = new List<GameObject>();
    private List<ReactCatMovement> _playerPoints = new List<ReactCatMovement>();
    private GameObject _fishClone;
    private int _roundNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void BeginMinigame()
    {
        foreach (Transform p in players)
        {
            p.GetComponent<ReactCatMovement>().StartMinigame();
        }
        StartCoroutine(MinigameLoop());
    }

    private IEnumerator MinigameLoop()
    {
        AddObjectsInList();
        while (_roundNumber < fishOrder.Count)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            yield return StartCoroutine(SpawnFish());
            SetPlayersFishOn();
            SetCPUHandlingOn();
            yield return new WaitForSeconds(timeToReact);
            yield return StartCoroutine(ShowPoints());
            SetPlayersFishOff();
            yield return StartCoroutine(MoveFishAway());

        }
    }

    private void AddObjectsInList()
    {
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < goodFishAmount; i++)
        {
            tempList.Add(fish);
        }

        for (int i = 0; i < badFishAmount; i++)
        {
            tempList.Add(badFish);
        }

        fishOrder = CreateConstrainedShuffle(tempList, 4);
    }

    List<GameObject> CreateConstrainedShuffle(List<GameObject> items, int maxConsecutive)
    {
        List<GameObject> shuffledList = new List<GameObject>();
        Dictionary<GameObject, int> objectCounts = new Dictionary<GameObject, int>();

        foreach (var obj in items)
        {
            if (!objectCounts.ContainsKey(obj))
                objectCounts[obj] = 0;
            objectCounts[obj]++;
        }

        while (shuffledList.Count < items.Count)
        {
            List<GameObject> possibleChoices = new List<GameObject>();

            foreach (var obj in objectCounts.Keys)
            {
                if (objectCounts[obj] > 0)
                {
                    if (shuffledList.Count < maxConsecutive || !IsLastNConsecutive(shuffledList, obj, maxConsecutive))
                    {
                        possibleChoices.Add(obj);
                    }
                }
            }

            if (possibleChoices.Count == 0)
            {
                possibleChoices.AddRange(objectCounts.Keys);
            }

            GameObject chosen = possibleChoices[Random.Range(0, possibleChoices.Count)];
            shuffledList.Add(chosen);
            objectCounts[chosen]--;
        }

        return shuffledList;
    }

    bool IsLastNConsecutive(List<GameObject> list, GameObject obj, int n)
    {
        if (list.Count < n) return false;

        for (int i = list.Count - n; i < list.Count; i++)
        {
            if (list[i] != obj) return false;
        }

        return true;
    }

    private IEnumerator SpawnFish()
    {

        _fishClone = Instantiate(fishOrder[_roundNumber], spawnPlace.position, Quaternion.identity);

        while (_fishClone.transform.position != middlePlace.position)
        {
            _fishClone.transform.position = Vector3.MoveTowards(_fishClone.transform.position, middlePlace.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        _roundNumber++;

    }

    private void SetCPUHandlingOn()
    {
        foreach (Transform p in players)
        {
            p.GetComponent<ReactCatMovement>().HandleFish(_fishClone.transform);
        } 
    }
    private void SetPlayersFishOn()
    {
        foreach (Transform p in players)
        {
            p.GetComponent<ReactCatMovement>().SetFishAvailableOn();
        }
    }

    private void SetPlayersFishOff()
    {
        foreach (Transform p in players)
        {
            p.GetComponent<ReactCatMovement>().SetFishAvailableOff();
        }
    }

    private IEnumerator MoveFishAway()
    {
        while (_fishClone.transform.position != endPlace.position)
        {
            _fishClone.transform.position = Vector3.MoveTowards(_fishClone.transform.position, endPlace.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(_fishClone);
    }

    public void AddPlayer(ReactCatMovement playerScript)
    {
        _playerPoints.Add(playerScript);
    }

    private IEnumerator ShowPoints()
    {
        if (_playerPoints.Count == 0) yield break;

        if (_fishClone.transform.CompareTag("BadFish"))
        {
            for (int i = 0; i < _playerPoints.Count; i++)
            {
                _playerPoints[i].AddPlayerPoints(-5);
            }
        }

        else
        {
            for (int i = 0; i < _playerPoints.Count; i++)
            {

                _playerPoints[i].AddPlayerPoints(pointAmounts[i]);
            }
        }
        

        yield return new WaitForSeconds(1);
        for (int i = 0; i < _playerPoints.Count; i++)
        {
            StartCoroutine(_playerPoints[i].MoveToOriginalPosition());
        }
        _playerPoints.Clear();
    }
}
