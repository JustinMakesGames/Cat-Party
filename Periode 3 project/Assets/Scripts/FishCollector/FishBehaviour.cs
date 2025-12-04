using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public int pointAmount;
    [HideInInspector] public bool isCollected;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float yForce; 
    [SerializeField] private Transform stopPlace;
    [SerializeField] private float disappearTime;
    private Rigidbody _rb;
    private bool _hasHitStopPlace;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.linearVelocity = new Vector3(0, -yForce, 0);
        Destroy(transform.parent.gameObject, 8f);

    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        if (Vector3.Distance(transform.position, stopPlace.position) < 0.2f && !_hasHitStopPlace)
        {
            _hasHitStopPlace = true;
            transform.position = stopPlace.position;
            StartCoroutine(StartToDisappear());
        }
      
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartToDisappear()
    {
        _rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(disappearTime);

        for (int i = 0; i < 10; i++)
        {
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(transform.parent.gameObject);
    }
}
