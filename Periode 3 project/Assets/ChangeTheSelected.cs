using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class ChangeTheSelected : MonoBehaviour
{
    private MultiplayerEventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = GetComponent<MultiplayerEventSystem>();
    }
    public void SetFirstSelected(GameObject selectedGameObject)
    {
        _eventSystem.SetSelectedGameObject(selectedGameObject);
    }
}
