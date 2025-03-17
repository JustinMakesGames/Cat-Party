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

    [SerializeField] private List<int> pointAmounts = new List<int>();
    private List<ReactCatMovement> _playerPoints = new List<ReactCatMovement>();
    private GameObject _fishClone;
    private int _fishAmount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void BeginMinigame()
    {
        StartCoroutine(MinigameLoop());
    }

    private IEnumerator MinigameLoop()
    {
        while (_fishAmount <= totalFishAmount)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            yield return StartCoroutine(SpawnFish());
            SetPlayersFishOn();
            yield return new WaitForSeconds(timeToReact);
            yield return ShowPoints();
            SetPlayersFishOff();
            yield return StartCoroutine(MoveFishAway());

        }
    }

    private IEnumerator SpawnFish()
    {
        _fishClone = Instantiate(fish, spawnPlace.position, Quaternion.identity);

        while (_fishClone.transform.position != middlePlace.position)
        {
            _fishClone.transform.position = Vector3.MoveTowards(_fishClone.transform.position, middlePlace.position, moveSpeed * Time.deltaTime);
            yield return null;
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
        for (int i = 0; i < _playerPoints.Count; i++)
        {
            _playerPoints[i].AddPlayerPoints(pointAmounts[i]);
        }

        yield return new WaitForSeconds(1);

        for (int i = 0; i < _playerPoints.Count; i++)
        {
            StartCoroutine(_playerPoints[i].MoveToOriginalPosition());
        }
        _playerPoints.Clear();
    }
}
