using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacesInstance : MonoBehaviour
{
    public static SpacesInstance Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
