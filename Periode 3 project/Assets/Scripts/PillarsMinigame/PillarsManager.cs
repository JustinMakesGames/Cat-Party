using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PillarsManager : MonoBehaviour, IMinigameManager
{
    public static PillarsManager Instance;
    public List<Transform> alivePlayers = new List<Transform>();
    public GameObject speedBoosterClone;
    [SerializeField] private Transform playerFolder;
    [SerializeField] private Transform platformFolder;
    [SerializeField] private Renderer signRenderer;
    [SerializeField] private float timeDown;
    [SerializeField] private float firstWait, secondWait, thirdWait;
    [SerializeField] private float shakeAmount;
    [SerializeField] private GameObject speedBoostPrefab;
    [SerializeField] private Transform arena;

    [SerializeField] private float platformSpeed;
    [SerializeField] private float platformWait;
    private float _firstInitialTime, _secondInitialTime, _thirdInitialTime;
    private Transform _rightPillar;
    private List<PlayerHandler> playerHandlers = new List<PlayerHandler>();
    private List<Transform> platforms = new List<Transform>();
    private List<Vector3> originalPlatformPositions = new List<Vector3>();

    private int _timesElapsed;
    private bool _isShaking;

    private bool _hasFinishedMinigame;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        _firstInitialTime = firstWait;
        _secondInitialTime = secondWait;
    }
    public void BeginMinigame()
    {
        AssignEverything();

        for (int i = 0; i < playerFolder.childCount; i++)
        {
            playerFolder.GetChild(i).GetComponent<PillarPlayerMovement>().StartMinigame();
        }

        StartCoroutine(ChoosePillar());
        StartCoroutine(SpawnSpeedBoosters());
    }

    private IEnumerator ChoosePillar()
    {
        while (!_hasFinishedMinigame)
        {
            yield return new WaitForSeconds(firstWait);
            ChooseRandomColor();
            TurnCPUSMovementOn();
            yield return new WaitForSeconds(secondWait);
            _isShaking = true;
            yield return new WaitForSeconds(thirdWait);
            _isShaking = false;
            PutPillarsToOriginalPosition();
            yield return StartCoroutine(PillarsGoDown());
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(PillarsGoUp());
            TurnCPUSMovementOff();
            TimeDown();
        }
        

    }

    private IEnumerator SpawnSpeedBoosters()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);

            if (speedBoosterClone == null) 
            {
                yield return new WaitForSeconds(2);
                ChoosePlaceToSpawn();
            }
            yield return null;
        }
        
    }

    private void ChoosePlaceToSpawn()
    {
        Vector3 position = SearchPosition();

        speedBoosterClone = Instantiate(speedBoostPrefab, position, Quaternion.identity);

    }

    private Vector3 SearchPosition()
    {
        Bounds bounds = arena.GetComponentInChildren<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x + 2, bounds.max.x - 2),
            transform.position.y,
            Random.Range(bounds.min.z + 2, bounds.max.z - 2));

        return destination;
    }

    private void Update()
    {
        LetPillarsShake();
    }
    private void ChooseRandomColor()
    {
        int randomPillar = Random.Range(0, platforms.Count);
        _rightPillar = platforms[randomPillar];
        signRenderer.material.color = _rightPillar.GetComponent<PlatformColor>().platformColor;
    }

    private void TurnCPUSMovementOn()
    {
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            StartCoroutine(playerFolder.GetChild(i).GetComponent<PillarPlayerMovement>().SetColorOn(_rightPillar));
        }
    }

    private void TurnCPUSMovementOff()
    {
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            playerFolder.GetChild(i).GetComponent<PillarPlayerMovement>().SetColorOff();
        }
    }

    private void PutPillarsToOriginalPosition()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].position = originalPlatformPositions[i];
        }
    }

    private void LetPillarsShake()
    {
        if (_isShaking)
        {
            for (int i = 0; i < platforms.Count; i++) 
            {
                if (platforms[i] == _rightPillar) continue;
                Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                0,
                Random.Range(-shakeAmount, shakeAmount)
                );

                platforms[i].position = originalPlatformPositions[i] + shakeOffset;
            }
        }
    }
    

    private void TimeDown()
    {
        firstWait = Mathf.Max(_firstInitialTime - (_timesElapsed * timeDown), 0.2f);
        secondWait = Mathf.Max(_secondInitialTime - (_timesElapsed * timeDown), 0.5f);
        thirdWait = Mathf.Max(_secondInitialTime - (_timesElapsed * timeDown), 0.5f);
        _timesElapsed++;
    }

    private IEnumerator PillarsGoDown()
    {
        float timer = 0;

        while (timer < platformWait)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i] == _rightPillar) continue;
                platforms[i].Translate(Vector3.down * platformSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    private IEnumerator PillarsGoUp()
    {
        float timer = 0;

        while (timer < platformWait)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i] == _rightPillar) continue;
                platforms[i].Translate(Vector3.up * platformSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }
    private void AssignEverything()
    {
        for (int i = 0; i < playerFolder.childCount; i++)
        {
            playerHandlers.Add(playerFolder.GetChild(i).GetComponent<MinigamePlayerHandler>().playerHandler);
        }

        for (int i = 0; i < platformFolder.childCount; i++)
        {
            platforms.Add(platformFolder.GetChild(i));
            originalPlatformPositions.Add(platformFolder.GetChild(i).position);
        }
    }

    public void HandleDeadPlayer(Transform player)
    {
        if (alivePlayers.Contains(player))
        {
            alivePlayers.Remove(player);
        }

        if (alivePlayers.Count == 1)
        {
            _hasFinishedMinigame = true;
            alivePlayers[0].GetComponent<PillarPlayerMovement>().StopAllCoroutines();
            alivePlayers[0].GetComponent<PillarPlayerMovement>().hasMinigameStart = false;
            MinigameManager.Instance.EndMinigame(alivePlayers[0].GetComponent<MinigamePlayerHandler>().playerHandler);
        }
    }
}
