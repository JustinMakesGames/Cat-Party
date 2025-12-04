using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBombHandling : MonoBehaviour
{ 
    [SerializeField] private float yForce;
    [SerializeField] private float explodeTime;
    [SerializeField] private GameObject explosionObject;
    private Rigidbody _rb;
    private Color _originalColor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.linearVelocity = new Vector3(0, -yForce, 0);
        _originalColor = GetComponent<Renderer>().material.color;
        StartCoroutine(FlickerColor());
        StartCoroutine(StartToExplode());
    }

    

    private IEnumerator StartToExplode()
    {
        yield return new WaitForSeconds(explodeTime);
        Instantiate(explosionObject, transform.position, Quaternion.identity);
        Destroy(transform.parent.gameObject);
    }
    private IEnumerator FlickerColor()
    {
        while (true)
        {
            GetComponent<Renderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            GetComponent<Renderer>().material.color = _originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
