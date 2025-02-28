using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    [SerializeField] private float speed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in allPlayers)
            {
                if (player == other.gameObject) continue;

                if (player.GetComponent<PillarPlayerMovement>() != null)
                {
                    player.GetComponent<PillarPlayerMovement>().CallChangeSpeed();
                }
                
            }
            
            Destroy(gameObject);
        }
    }
}
