using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuMovementScript : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody _rb;
    private Vector3 _dir;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void OnMovement(InputAction.CallbackContext context) 
    {
        _dir.x = context.ReadValue<Vector2>().x;
        _dir.z = context.ReadValue<Vector2>().y;
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * speed * Time.deltaTime;
    }
}
