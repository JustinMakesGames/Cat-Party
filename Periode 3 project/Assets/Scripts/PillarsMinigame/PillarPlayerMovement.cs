using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PillarPlayerMovement : MonoBehaviour
{
    public enum CPUState
    {
        Nothing,
        GunSpawned,
        HasGun,
        RunningFromGun
    }

    public CPUState state;
    public bool hasMinigameStart;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rayCastLength;
    [SerializeField] private LayerMask ground;
    

    [Header("Player Variables")]
    private float xValue;
    private float yValue;
    private Vector3 _dir;
    private bool _isPlayer;
    private PlayerInput _playerInput;
    private Rigidbody _rb;
    private RaycastHit hit;

    [Header("CPU Variables")]
    public Transform rightArena;
    public bool isColorRevealed;
    [SerializeField] private Transform normalArena;
    private bool hasReachedArea = true;
    private Vector3 _destination;
    private Vector3 cpuDirection;
    private bool _isIdle;
    
    
    
    
    private void Start()
    {
        
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>(); 
    }
    public void StartMinigame()
    {
        hasMinigameStart = true;

        if (_playerInput.enabled)
        {
            _isPlayer = true;
        }
    }

    
    private void FixedUpdate()
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

    public void ControlPlayerMovement(InputAction.CallbackContext context)
    {
        xValue = context.ReadValue<Vector2>().x;
        yValue = context.ReadValue<Vector2>().y;
    }
    private void HandlePlayerMovement()
    {
        _rb.velocity = new Vector3(xValue * walkSpeed * Time.deltaTime, _rb.velocity.y, yValue * walkSpeed * Time.deltaTime);
    }
   
    public IEnumerator SetColorOn(Transform rightColorPlatform)
    {
        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
        hasReachedArea = true;
        isColorRevealed = true;
        rightArena = rightColorPlatform;
        StartCoroutine(HandleColorRevealedMovement());
    }

    public void SetColorOff()
    {
        hasReachedArea = true;
        isColorRevealed = false;
        _isIdle = false;
        StopAllCoroutines();
    }
    private void HandleCPU()
    {

        if (!isColorRevealed)
        {
            HandleNormalCPUMovement();
        }

        if (_isIdle) return;
        cpuDirection = _destination - _rb.position;
        cpuDirection.Normalize();

        _rb.velocity = new Vector3(cpuDirection.x * walkSpeed * Time.deltaTime, _rb.velocity.y, cpuDirection.z * walkSpeed * Time.deltaTime);
    }

    private IEnumerator HandleColorRevealedMovement()
    {
        while (true)
        {
            if (hasReachedArea)
            {
                _destination = CalculateColorDestination();
                hasReachedArea = false;
            }

            if (Vector3.Distance(transform.position, _destination) < 0.1f)
            {
                _isIdle = true;
                _rb.velocity = Vector3.zero;
                yield return new WaitForSeconds(1);
                _isIdle = false;
                hasReachedArea = true;
            }

            yield return null;
        }
        
    }

    private Vector3 CalculateColorDestination()
    {
        Bounds bounds = rightArena.GetComponent<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x + 2, bounds.max.x - 2),
            transform.position.y,
            Random.Range(bounds.min.z + 2, bounds.max.z - 2));

        return destination;
    }

    private void HandleNormalCPUMovement()
    {
        if (hasReachedArea)
        {
            _destination = CalculateNormalDestination();
            hasReachedArea = false;
        }

        if (Vector3.Distance(transform.position, _destination) < 0.1f)
        {
            hasReachedArea = true;
        }
    }

    private Vector3 CalculateNormalDestination()
    {
        Bounds bounds = normalArena.GetComponent<Collider>().bounds;
        Vector3 destination = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z));

        return destination;
    }


}
