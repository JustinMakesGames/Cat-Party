using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReactCatMovement : MonoBehaviour
{
    public int points;
    [SerializeField] private Transform paw;
    [SerializeField] private Transform endPosition;
    [SerializeField] private float speed;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text plusPointText;
    [SerializeField] private TMP_Text totalPointsText;
    private Vector3 _originalPosition;
    private bool _isFishAvailable;
    private bool _isMovingPaw;

    //CPU Variables
    private bool _isCPU;

    private void Awake()
    {
        _originalPosition = paw.position;
    }

    public void StartMinigame()
    {
        if (!GetComponent<PlayerInput>().enabled)
        {
            _isCPU = true;
        }
    }
    public void GetFish(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!_isFishAvailable && !_isMovingPaw)
            {
                StartCoroutine(HandleNoAvailableFish());
            }

            else if (_isFishAvailable && !_isMovingPaw)
            {
                StartCoroutine(HandleAvailableFish());
            }
        }
        
    }

       
    

    public void HandleFish(Transform fish)
    {
        if (!_isCPU) return;

        print("This is a cpu");
        if (fish.CompareTag("BadFish"))
        {
            HandleBadFish();
        }

        else
        {
            StartCoroutine(HandleCPUTakingFish());
        }
    }

    private void HandleBadFish()
    {
        int randomChance = Random.Range(0, 3);

        if (randomChance == 0)
        {
            StartCoroutine(HandleCPUTakingFish());
        }
    }

    private IEnumerator HandleCPUTakingFish()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
        if (!_isFishAvailable && !_isMovingPaw)
        {
            StartCoroutine(HandleNoAvailableFish());
        }

        else if (_isFishAvailable && !_isMovingPaw)
        {
            StartCoroutine(HandleAvailableFish());
        }

    }
    private IEnumerator HandleNoAvailableFish()
    {
        _isMovingPaw = true;
        yield return StartCoroutine(MoveToMiddle());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveToOriginalPosition());
        yield return new WaitForSeconds(0.2f);
        _isMovingPaw = false;
    }

    private IEnumerator HandleAvailableFish()
    {
        _isMovingPaw = true;
        ReactCatsManager.Instance.AddPlayer(this);
        yield return StartCoroutine(MoveToMiddle());

    }
    private IEnumerator MoveToMiddle()
    {
        while (paw.position != endPosition.position)
        {
            paw.position = Vector3.MoveTowards(paw.position, endPosition.position, speed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator MoveToOriginalPosition()
    {
        while (paw.position != _originalPosition)
        {
            paw.position = Vector3.MoveTowards(paw.position, _originalPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void SetFishAvailableOn()
    {
        _isFishAvailable = true;
    }

    public void SetFishAvailableOff()
    {
        _isFishAvailable = false;
        _isMovingPaw = false;
    }

    public void AddPlayerPoints(int point)
    {
        points += point;

        if (points < 0) points = 0;
        totalPointsText.text = points.ToString();
        StartCoroutine(ShowPoint(point));
    }

    private IEnumerator ShowPoint(int point)
    {
        canvas.SetActive(true);
        plusPointText.text = point.ToString();
        yield return new WaitForSeconds(1);
        canvas.SetActive(false);
    }
}
