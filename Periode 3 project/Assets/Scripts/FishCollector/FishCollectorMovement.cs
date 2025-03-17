using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.LightAnchor;
using UnityEngine.Rendering.Universal;

public class FishCollectorMovement : MinigamePlayerMovement
{
    public int points;
    [SerializeField] private GameObject canvas;

    //CPU Variables
    [SerializeField] private Transform fishFolder;
    [SerializeField] private Transform targetFish;
    [SerializeField] private Transform normalArena;
    private Vector3 _destination;
    private Vector3 _cpuDirection;
    private bool _isSearchingFish;
    

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish") && !other.GetComponent<FishBehaviour>().isCollected)
        {
            other.GetComponent<FishBehaviour>().isCollected = true;
            points += other.GetComponent<FishBehaviour>().pointAmount;
            Destroy(other.transform.parent.gameObject);
            StartCoroutine(ShowPoints(other.GetComponent<FishBehaviour>().pointAmount));
        }
    }

    public override void StartMinigame()
    {
        hasMinigameStart = true;

        if (_playerInput.enabled)
        {
            _isPlayer = true;
        }

        else
        {
            StartCoroutine(HandleSearchingForTarget());
        }


    }

    private IEnumerator HandleSearchingForTarget()
    {
        while (true)
        {
            if (targetFish == null)
            {
                SearchForNewTargetFish();
            }
            yield return new WaitForSeconds(0.5f);
        }
        

        
    }

    protected override void FixedUpdate()
    {
        if (hasMinigameStart)
        {
            if (_isPlayer)
            {
                HandlePlayerMovement();
            }

            else
            {
                HandleCPU();
            }
        }
    }

    private void HandleCPU()
    {
        if (targetFish == null)
        {
            if (!_isSearchingFish || Vector3.Distance(transform.position, _destination) < 0.1f)
            {
                _destination = SearchNormalNewDestination();
                _isSearchingFish = true;
            }
        }

        else
        {
            _isSearchingFish = false;
        }

        _cpuDirection = _destination - _rb.position;
        _cpuDirection.Normalize();
        _rb.velocity = new Vector3(_cpuDirection.x * walkSpeed * Time.deltaTime, _rb.velocity.y, _cpuDirection.z * walkSpeed * Time.deltaTime);
        RotationCheck();
    }

    private void SearchForNewTargetFish()
    {
        if (fishFolder.childCount == 0) return;
        int index = -1;
        float smallestDistance = Mathf.Infinity;

        for (int i = 0; i < fishFolder.childCount; i++) 
        {
            float distance = Vector3.Distance(transform.position, new Vector3(fishFolder.GetChild(i).position.x, transform.position.y, fishFolder.GetChild(i).position.z));

            if (distance < smallestDistance)
            {
                if (fishFolder.GetChild(i).name == "PurpleFish")
                {
                    int randomChance = Random.Range(0, 3);

                    if (randomChance != 0) continue; 
                }
                smallestDistance = distance;
                index = i;
            }
        }

        targetFish = fishFolder.GetChild(index);

        _destination = targetFish.position;
    }

    private Vector3 SearchNormalNewDestination()
    {
        Bounds bounds = normalArena.GetComponentInChildren<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z));

        return destination;
    }
    private IEnumerator ShowPoints(int pointAmount)
    {
        canvas.SetActive(true);

        if (pointAmount < 0)
        {
            canvas.GetComponentInChildren<TMP_Text>().text = pointAmount.ToString();
        }

        else
        {
            canvas.GetComponentInChildren<TMP_Text>().text = "+" + pointAmount.ToString();
        }

        yield return new WaitForSeconds(1);
        canvas.SetActive(false);
        
    }

    public void MinigameEnded()
    {
        StopAllCoroutines();
        _rb.velocity = Vector3.zero;
        hasMinigameStart = false;
    }


}
