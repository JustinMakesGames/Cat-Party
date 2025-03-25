using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SweeperManager : MonoBehaviour, IMinigameManager
{
    public static SweeperManager Instance;
    [Header("Cylinder handling")]
    [SerializeField] private List<Transform> players = new List<Transform>();
    [SerializeField] private Transform cylinderCenter;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float timeForNewCylinder;
    [SerializeField] private float scaleSpeed;

    [Header("BombHandling")]
    [SerializeField] private GameObject bomb;
    [SerializeField] private Transform arena;
    [SerializeField] private float bombSpawningInterval;
    [SerializeField] private float bombIntervalAcceleration;
    private Bounds _arenaBounds;

    private int _placement = 4;
    private Transform cylinderToScale;


    private bool _hasGameStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void BeginMinigame()
    {
        _arenaBounds = arena.GetComponent<Collider>().bounds;
        foreach (Transform t in players)
        {
            t.GetComponent<MinigamePlayerMovement>().StartMinigame();
        }


        _hasGameStarted = true;
        StartCoroutine(SpawnBombs());
    }

    private void Update()
    {
        if (_hasGameStarted)
        {
            currentSpeed += accelerationSpeed * Time.deltaTime;
            cylinderCenter.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
            bombSpawningInterval -= bombIntervalAcceleration * Time.deltaTime;
        }
    }

   

    /*private IEnumerator NewCylinderAnimation(int index, bool isOneSided, float maxScaleY)
    {
        cylinderToScale = cylinderCenter.GetChild(index);
        while (cylinderToScale.localScale.y < maxScaleY)
        {
            cylinderToScale.localScale += new Vector3(0, scaleSpeed * Time.deltaTime, 0);

            if (isOneSided)
            {
                cylinderToScale.position += cylinderToScale.up * (scaleSpeed * Time.deltaTime);
            }
            
            yield return null;
        }


    }*/

    private IEnumerator SpawnBombs()
    {
        while (true)
        {
            Vector3 position = GetBombPosition();
            Instantiate(bomb, position, Quaternion.identity);
            yield return new WaitForSeconds(bombSpawningInterval);
        }   
    }

    private Vector3 GetBombPosition()
    {
        Vector3 spawnPos = new Vector3(Random.Range(_arenaBounds.min.x, _arenaBounds.max.x), _arenaBounds.max.y,
            Random.Range(_arenaBounds.min.z, _arenaBounds.max.z));

        return spawnPos;
    }

    public void RemovePlayerFromList(Transform player)
    {
        if (players.Count == 1) return;
        players.Remove(player);

        MinigameManager.Instance.ThrowPlayerInDictionary(player.GetComponent<MinigamePlayerHandler>().playerHandler, _placement);
        _placement--;
        if (players.Count == 1)
        {
            MinigameManager.Instance.ThrowPlayerInDictionary(players[0].GetComponent<MinigamePlayerHandler>().playerHandler, _placement);
            StopAllCoroutines();
            _hasGameStarted = false;
            MinigameManager.Instance.EndMinigame();
        }
    }

}
