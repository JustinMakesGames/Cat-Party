using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamePlayerMovement : MonoBehaviour
{
    public bool hasMinigameStart;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float rayCastLength;
    [SerializeField] protected LayerMask ground;
    [SerializeField] protected float normalSpeed;
    [SerializeField] protected float slowSpeed;
    [SerializeField] protected float pushForce;
    [SerializeField] protected float hitJumpForce;
    [SerializeField] protected int flickerAmount;
    protected bool _isPlayerPushing;
    protected bool _isHit;
    protected bool _isStunned;

    [Header("Player Variables")]
    protected float xValue;
    protected float yValue;
    protected bool _isPlayer;
    protected PlayerInput _playerInput;
    protected Rigidbody _rb;

    protected Animator animator;

    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void StartMinigame()
    {
        hasMinigameStart = true;

        if (_playerInput.enabled)
        {
            _isPlayer = true;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (hasMinigameStart && _isPlayer)
        {
            HandlePlayerMovement();
        }
    }

    private void Update()
    {
        animator.SetFloat("IsWalking", _rb.velocity.magnitude);
    }

    public void ControlPlayerMovement(InputAction.CallbackContext context)
    {
        xValue = context.ReadValue<Vector2>().x;
        yValue = context.ReadValue<Vector2>().y;
    }

    public void Push(InputAction.CallbackContext context)
    {
        if (context.performed && !_isPlayerPushing)
        {
            StartCoroutine(PlayerPushes());
        }
    }

    protected virtual IEnumerator PlayerPushes()
    {
        _isPlayerPushing = true;
        yield return new WaitForSeconds(0.2f);
        _isPlayerPushing = false;
    }

    protected virtual void GetHit(Transform player)
    {
        StartCoroutine(HitDelay());
    }

    protected virtual IEnumerator HitDelay()
    {
        Color originalColor = GetComponentInChildren<Renderer>().material.color;
        _isHit = true;
        _isStunned = true;
        GetComponentInChildren<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<Renderer>().material.color = originalColor;
        _isHit = false;
        yield return StartCoroutine(Flicker());
        _isStunned = false;
    }

    protected virtual IEnumerator Flicker()
    {
        for (int i = 0; i < flickerAmount; i++)
        {
            GetComponentInChildren<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            GetComponentInChildren<Renderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    protected virtual void HandlePlayerMovement()
    {
        if (!_isHit)
        {
            _rb.velocity = new Vector3(xValue * walkSpeed * Time.deltaTime, _rb.velocity.y, yValue * walkSpeed * Time.deltaTime);
        }
        RotationCheck();
    }

    public bool StunCheck()
    {
        return _isStunned;
    }

    protected virtual void RotationCheck()
    {
        if (_rb.velocity.x != 0 || _rb.velocity.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_rb.velocity.normalized);
            transform.rotation = targetRotation;
        }
    }

    public void CallChangeSpeed()
    {
        StartCoroutine(ChangeSpeed());
    }

    protected virtual IEnumerator ChangeSpeed()
    {
        walkSpeed = slowSpeed;
        yield return new WaitForSeconds(3);
        walkSpeed = normalSpeed;
    }

    protected virtual void OnTriggerEnter(Collider hitInfo)
    {
        if (!_isPlayerPushing) return;
        if (!hitInfo.transform.CompareTag("Player")) return;
        if (hitInfo.transform.GetComponent<HotPotatoMovement>().StunCheck()) return;

        hitInfo.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Vector3 pushDirection = (hitInfo.transform.position - _rb.position).normalized;
        pushDirection.y = hitJumpForce;
        hitInfo.transform.GetComponent<Rigidbody>().velocity = pushDirection * pushForce;
        hitInfo.transform.GetComponent<MinigamePlayerMovement>().GetHit(transform);
    }
}
