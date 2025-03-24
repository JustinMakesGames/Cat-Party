using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMapHandling : MonoBehaviour
{
    [SerializeField] private Transform endPosition;
    [SerializeField] private float beginSpeed;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float endSpeed;
    [SerializeField] private Transform areaToMoveAround;
    [SerializeField] private Transform originalPosition;
    [SerializeField] private GameObject chooseScreen;

    private Bounds _areaBounds;
    private Transform _cam;
    private bool _isLookingMap;

    private bool _isAtBrancingPath;
    private Vector3 _dir;
    
    public void StartMapHandling(bool isAtBranchingPath)
    {
        _isAtBrancingPath = isAtBranchingPath;
        areaToMoveAround = GameObject.FindGameObjectWithTag("AreaBounds").transform;
        chooseScreen.SetActive(false);
        _areaBounds = areaToMoveAround.GetComponent<Collider>().bounds;
        _cam = Camera.main.transform;
        StartCoroutine(MoveCameraUp());
    }

    public void CancelMapLooking(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isLookingMap)
            {
                
                StartCoroutine(StartCancellingMapHandling());
            }
        }
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        if (_isLookingMap)
        {
            _dir.x = context.ReadValue<Vector2>().x;
            _dir.z = context.ReadValue<Vector2>().y;
        }
    }
    private void LateUpdate()
    {
        if (_isLookingMap)
        {
            HandleMovingAround();
        }
    }

    
    private void HandleMovingAround()
    {
        Vector3 position = _cam.position + (_dir * cameraSpeed * Time.deltaTime);

        position.x = Mathf.Clamp(position.x, _areaBounds.min.x, _areaBounds.max.x);
        position.z = Mathf.Clamp(position.z, _areaBounds.min.z, _areaBounds.max.z);

        _cam.position = position;


    }
    private IEnumerator MoveCameraUp()
    {
        while (_cam.position != endPosition.position)
        {
            _cam.position = Vector3.MoveTowards(_cam.position, endPosition.position, beginSpeed * Time.deltaTime);
            
            yield return null;
        }

        _cam.rotation = endPosition.rotation;

        _isLookingMap = true;
    }

    private IEnumerator StartCancellingMapHandling()
    {
        _isLookingMap = false;
        yield return StartCoroutine(MoveCameraBack());

        if (_isAtBrancingPath)
        {
            GetComponent<HandleBrancingPathPlayer>().HandleCancelMap();
        }

        else
        {
            chooseScreen.SetActive(true);
        }
        

    }
    private IEnumerator MoveCameraBack()
    {
        while (_cam.position != originalPosition.position)
        {
            _cam.position = Vector3.MoveTowards(_cam.position, originalPosition.position, beginSpeed * Time.deltaTime);

            yield return null;
        }

        _cam.rotation = originalPosition.rotation;
    }
}
