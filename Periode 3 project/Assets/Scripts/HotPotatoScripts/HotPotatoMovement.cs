using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class HotPotatoMovement : MinigamePlayerMovement
{
    public Transform playerWithBomb;
    public Transform bomb;
    public bool hasBomb;
    

    [SerializeField] private Transform location;
    [SerializeField] private float fleeDistance;
    [SerializeField] private float playerDistance;
    [SerializeField] private Transform arena;
    [SerializeField] private float distance;

    private bool _isInCountdownBomb;

    private bool _isMovingFromWall;
    private float _range;
    
    private bool hasChangedState;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private bool _hasCoolDown;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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
            _agent.enabled = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!_agent.enabled) return;
        if (hasBomb)
        {
            FollowPlayer();
            return;
        }

        if (playerWithBomb == null) return;
        if (Vector3.Distance(transform.position, playerWithBomb.position) < playerDistance)
        {
            hasChangedState = false;
            EscapePlayer();
        }

        else if (!_isMovingFromWall && Vector3.Distance(transform.position, playerWithBomb.position) > playerDistance + 2)
        {
            RunAround();
        }
    }

    private void FollowPlayer()
    {
        _destination = CalculatePlayerPosition();
        _agent.SetDestination(_destination);
    }

    private void RunAround()
    {  
        if (Vector3.Distance(transform.position, _destination) < 0.1f || !hasChangedState)
        {
            hasChangedState = true;
            _destination = CalculateNormalPosition();
            _agent.SetDestination(_destination);
        }
    }

    private void EscapePlayer()
    {
        _agent.SetDestination(CalculateRunFromPlayerDestination());
    }
    protected override void OnTriggerEnter(Collider hitInfo)
    {
        if (hitInfo.CompareTag("Player") && hasBomb && !_isInCountdownBomb)
        {
            if (hitInfo.GetComponent<HotPotatoMovement>().HasCoolDown()) return;
            hitInfo.GetComponent<HotPotatoMovement>().GetBombed(bomb);
            hasBomb = false;
            StartCoroutine(CoolDownCountdown());
        }
    }

    private IEnumerator CoolDownCountdown()
    {
        _hasCoolDown = true;
        yield return new WaitForSeconds(3);
        _hasCoolDown = false;
    }

    public void GetBombed(Transform bomb)
    {
        bomb.position = transform.GetChild(0).position;
        bomb.parent = transform;

        this.bomb = bomb;

        walkSpeed = 600;
        _agent.speed = 14f;
        foreach (Transform p in transform.parent)
        {
            if (p == transform) continue;
            p.GetComponent<HotPotatoMovement>().HandleBombPlayerChange(transform);
        }

        StartCoroutine(WaitToReactHavingBomb());
    }

    private IEnumerator WaitToReactHavingBomb()
    {
        yield return new WaitForSeconds(1.5f);
        hasBomb = true;
    }

    public void HandleBombPlayerChange(Transform bombedPlayer)
    {
        _agent.speed = 11f;
        walkSpeed = 500;
        if (_agent.enabled)
        {
            playerWithBomb = bombedPlayer;
            StopCoroutine(ChangeRange());
            StartCoroutine(ChangeRange());
        }
       
    }

    private IEnumerator ChangeRange()
    {        
        while (true)
        {
            _range = RangeCalculation();
            yield return new WaitForSeconds(1);
        }
    }

    private Vector3 CalculateRunFromPlayerDestination()
    {
        Vector3 awayDirection = (transform.position - playerWithBomb.position).normalized;

        awayDirection = Quaternion.Euler(0, _range, 0) * awayDirection;
        Vector3 destination = transform.position + awayDirection * fleeDistance;

        _destination = destination;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, distance, NavMesh.AllAreas))
        {
            print(transform.name + " hit the ground luckily");
            return hit.position;
        }

        StartCoroutine(WaitToMoveToOtherPlace());
        return CalculateNormalPosition();

        
    }

    private IEnumerator WaitToMoveToOtherPlace()
    {
        _isMovingFromWall = true;
        yield return new WaitForSeconds(1);
        _isMovingFromWall = false;
    }

    private Vector3 CalculateNormalPosition()
    {
        Bounds bounds = arena.GetComponent<Collider>().bounds;

        Vector3 randomPosition = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y, Random.Range(bounds.min.z, bounds.max.z));

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    public bool HasCoolDown()
    {
        return _hasCoolDown;
    }

    private float RangeCalculation()
    {
        float randomValue = Random.Range(-90f, 90f);

        return randomValue;
    }

    private Vector3 CalculatePlayerPosition()
    {
        int closestPlayer = 0;
        float smallestDistance = Mathf.Infinity;
        foreach (Transform t in transform.parent)
        {
            if (t == transform) continue;

            if (t.GetComponent<HotPotatoMovement>().HasCoolDown()) continue;

            float distance = Vector3.Distance(transform.position, t.position);
            if (distance < smallestDistance)
            {
                closestPlayer = t.GetSiblingIndex();
                smallestDistance = distance;
            }

        }

        if (NavMesh.SamplePosition(transform.parent.GetChild(closestPlayer).position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return CalculateNormalPosition();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_destination, distance); // Intended destination

        
    }
}
