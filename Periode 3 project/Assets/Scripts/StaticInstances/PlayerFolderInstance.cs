using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFolderInstance : MonoBehaviour
{
    public static PlayerFolderInstance Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
