using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManaging : MonoBehaviour
{
    public void MoveToMenu()
    {
        Scene newScene = SceneManager.CreateScene("DeleteScene");

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == "DontDestroyOnLoad" && obj.transform.parent == null)
            {
                SceneManager.MoveGameObjectToScene(obj, newScene);
            }
        }

        SceneManager.UnloadSceneAsync(newScene);
        SceneManager.LoadScene("Menu");
    }
}
