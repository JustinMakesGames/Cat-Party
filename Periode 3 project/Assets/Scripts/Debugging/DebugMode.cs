using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMode : MonoBehaviour
{
    private bool _isDebuggingMinigames;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += DebugProcess;
    }

    private void DebugProcess(Scene scene, LoadSceneMode loadMode)
    {
        if (SceneManager.GetActiveScene().name != "BoardGame") return;
        if (_isDebuggingMinigames)
        {
            GameObject.FindGameObjectWithTag("GameCanvas").SetActive(false);
            BoardGameManager.Instance.DebuggingOn();
        }

        else
        {
            GameObject.FindGameObjectWithTag("GameCanvas").SetActive(true);
        }
        
    }
}
