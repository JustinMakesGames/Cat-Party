using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private bool _isColliderGone;
    private void Start()
    {
        StartCoroutine(Explosion());
        Destroy(gameObject, 1);
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.8f);
        _isColliderGone = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isColliderGone)
        {
            other.GetComponent<SweeperPlayerController>().TakeDamage();
        }
    }
}
