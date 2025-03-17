using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Vector3 _originalPosition;
    private bool _isFishAvailable;
    private bool _isMovingPaw;

    private void Awake()
    {
        _originalPosition = paw.position;
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

    private IEnumerator HandleNoAvailableFish()
    {
        _isMovingPaw = true;
        yield return StartCoroutine(MoveToMiddle());
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(MoveToOriginalPosition());
        _isMovingPaw = false;
    }

    private IEnumerator HandleAvailableFish()
    {
        _isMovingPaw = true;
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
    }

    public void AddPlayerPoints(int point)
    {
        points += point;
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
