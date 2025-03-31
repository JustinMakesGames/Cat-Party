using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuMovementScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Animator _animator;
    private Rigidbody _rb;
    private Vector3 _dir;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        SceneManager.sceneLoaded += SceneSwitchHandling;
    }
    public void OnMovement(InputAction.CallbackContext context) 
    {
        _dir.x = context.ReadValue<Vector2>().x;
        _dir.z = context.ReadValue<Vector2>().y;
    }

    private void Update()
    {
        _animator.SetFloat("IsWalking", _dir.magnitude);
        RotationCheck();
    }
    private void FixedUpdate()
    {
        _rb.velocity = _dir * speed * Time.deltaTime;
    }

    private void RotationCheck()
    {
        if (_dir.x != 0 || _dir.z != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_dir.normalized);

            transform.rotation = targetRotation;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneSwitchHandling;
    }
    private void SceneSwitchHandling(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DeleteObject());
    }

    private IEnumerator DeleteObject()
    {
        yield return null;
        Destroy(gameObject);
    }
}
