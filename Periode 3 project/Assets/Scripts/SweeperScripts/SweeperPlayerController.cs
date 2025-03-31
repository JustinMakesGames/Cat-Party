using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.LightAnchor;

public class SweeperPlayerController : MinigamePlayerMovement
{
    [SerializeField] private float gravity;
    [SerializeField] private Transform arena;
    [SerializeField] private float distanceFromBomb;
    [SerializeField] private float distanceToWalk;
    [SerializeField] private TMP_Text livesText;

    [SerializeField] private int lives;
    private bool _isInvincible;
    private Color _color;

    private Vector3 _destination;
    private Vector3 _cpuDirection;
    private bool _hasReachedArea;
    private Animator _animator;




    private void Awake()
    {
        _color = GetComponentInChildren<Renderer>().material.color;
        _animator = GetComponentInChildren<Animator>();
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        _destination = CalculateNormalDestination();

        if (!_isPlayer)
        {
            StartCoroutine(HandleNormalCPUMovement());
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }
    public void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayCastLength, ground))
        {
            _animator.SetTrigger("Jump");
            _rb.velocity = new Vector3(_rb.velocity.x, jumpForce, _rb.velocity.z);
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
        _rb.AddForce(new Vector3(0, -gravity, 0), ForceMode.Acceleration);
    }

    private void HandleCPU()
    {
        if (Vector3.Distance(transform.position, _destination) < 0.1f)
        {
            _hasReachedArea = true;
        }

        if (!_hasReachedArea)
        {
            _cpuDirection = _destination - _rb.position;
            _cpuDirection.Normalize();
            _rb.velocity = new Vector3(_cpuDirection.x * walkSpeed * Time.deltaTime, _rb.velocity.y, _cpuDirection.z * walkSpeed * Time.deltaTime);
            RotationCheck();
        } 

        else
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("JumpCollider") && !_isPlayer)
        {
            Jump();
        }

        if (other.CompareTag("Bomb") && !_isPlayer) 
        {
            _hasReachedArea = false;
            print("AAAAAA A BOMB! RUN PLEASE");
            _destination = CalculateOppositeBomb(other.transform);
        } 
    }

    public void TakeDamage()
    {
        if (_isInvincible) return;
        
        lives--;
        livesText.text = lives.ToString();

        if (lives == 0)
        {
            SweeperManager.Instance.RemovePlayerFromList(transform);
            Destroy(gameObject);
            return;
        }

        StartCoroutine(HandleDamage());
    }

    private IEnumerator HandleDamage()
    {
        _isInvincible = true;
        GetComponentInChildren<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        GetComponentInChildren<Renderer>().material.color = _color;

        for (int i = 0; i < 5; i++)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            yield return new WaitForSeconds(0.1f);

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }

            yield return new WaitForSeconds(0.1f);
        }

        _isInvincible = false;
    }

    private IEnumerator HandleNormalCPUMovement()
    {
        while (true)
        {
            _hasReachedArea = false;
            _destination = CalculateNormalDestination();

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
        
    }

    private Vector3 CalculateNormalDestination()
    {
        Bounds bounds = arena.GetComponent<Collider>().bounds;
        Vector3 destination = transform.position + new Vector3(Random.Range(-distanceToWalk, distanceToWalk), 0, Random.Range(-distanceToWalk, distanceToWalk));

        destination.x = Mathf.Clamp(destination.x, bounds.min.x, bounds.max.x);
        destination.z = Mathf.Clamp(destination.z, bounds.min.z, bounds.max.z);

        return destination;
    }

    private Vector3 CalculateOppositeBomb(Transform bomb)
    {
        Bounds bounds = arena.GetComponent<Collider>().bounds;
        Vector3 direction = (transform.position - bomb.position).normalized;

        Vector3 destination = transform.position + (direction * distanceFromBomb);
        destination.y = transform.position.y;

        destination.x = Mathf.Clamp(destination.x, bounds.min.x, bounds.max.x);
        destination.z = Mathf.Clamp(destination.z, bounds.min.z, bounds.max.z);

        return destination;
    }
}
