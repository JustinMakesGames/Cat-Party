using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarsDeathPlain : MonoBehaviour
{
    [SerializeField] private PillarsManager manager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.HandleDeadPlayer(other.transform);
            Destroy(other.gameObject);
        }
    }
}
