using JetBrains.Annotations;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct TextStrings
{
    public string name;
    public List<string> strings;
}
public class TextListScript : MonoBehaviour
{
    public static TextListScript Instance;
    public List<TextStrings> textStrings = new List<TextStrings>();
    [SerializeField] private Transform playerFolder;
    [SerializeField] private TMP_Text gameText;

    private List<PlayerHandler> _playerHandlers = new List<PlayerHandler>();
    private bool _hasPressedButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < playerFolder.childCount; i++)
        {
            _playerHandlers.Add(playerFolder.GetChild(i).GetComponent<PlayerHandler>());
        }
        
    }

    public IEnumerator ShowPrompts(List<string> textPrompts)
    {
        gameText.transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < textPrompts.Count; i++)
        {
            foreach (PlayerHandler handler in _playerHandlers)
            {
                handler.canClickThroughText = true;
            }
            _hasPressedButton = false;
            gameText.text = textPrompts[i];
            yield return StartCoroutine(Delay(() => _hasPressedButton));

            foreach (PlayerHandler handler in _playerHandlers)
            {
                handler.canClickThroughText = false;
            }

        }
        gameText.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator ShowPrompts(List<string> textPrompts, bool continuesAutomatically)
    {
        gameText.transform.parent.gameObject.SetActive(true);

        if (continuesAutomatically)
        {
            for (int i = 0; i < textPrompts.Count; i++)
            {

                _hasPressedButton = false;
                gameText.text = textPrompts[i];
                yield return new WaitForSeconds(2);

            }
        }

        else
        {
            for (int i = 0; i < textPrompts.Count; i++)
            {
                foreach (PlayerHandler handler in _playerHandlers)
                {
                    handler.canClickThroughText = true;
                }
                _hasPressedButton = false;
                gameText.text = textPrompts[i];
                yield return StartCoroutine(Delay(() => _hasPressedButton));

                foreach (PlayerHandler handler in _playerHandlers)
                {
                    handler.canClickThroughText = false;
                }

            }
        }

        gameText.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator ShowPrompts(List<string> textPrompts, PlayerHandler specificPlayer)
    {
        gameText.transform.parent.gameObject.SetActive(true);

        for (int i = 0; i < textPrompts.Count; i++)
        {
            specificPlayer.canClickThroughText = true;
            _hasPressedButton = false;
            gameText.text = textPrompts[i];
            yield return StartCoroutine(Delay(() => _hasPressedButton));
            specificPlayer.canClickThroughText = false;

        }


        gameText.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator ShowPrompt(string textPrompt)
    {
        gameText.transform.parent.gameObject.SetActive(true);

        foreach (PlayerHandler handler in _playerHandlers)
        {
            handler.canClickThroughText = true;
        }
        _hasPressedButton = false;
        gameText.text = textPrompt;
        yield return StartCoroutine(Delay(() => _hasPressedButton));

        foreach (PlayerHandler handler in _playerHandlers)
        {
            handler.canClickThroughText = false;
        }
        gameText.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator ShowPrompt(string textPrompt, bool continuesAutomatically)
    {
        gameText.transform.parent.gameObject.SetActive(true);

        if (continuesAutomatically)
        {
            _hasPressedButton = false;
            gameText.text = textPrompt;
            yield return new WaitForSeconds(2);
        }

        else
        {
            foreach (PlayerHandler handler in _playerHandlers)
            {
                handler.canClickThroughText = true;
            }
            _hasPressedButton = false;
            gameText.text = textPrompt;
            yield return StartCoroutine(Delay(() => _hasPressedButton));

            foreach (PlayerHandler handler in _playerHandlers)
            {
                handler.canClickThroughText = false;
            }
        }

        gameText.transform.parent.gameObject.SetActive(false);
    }

    public IEnumerator ShowPrompt(string textPrompt, PlayerHandler specificPlayer)
    {
        gameText.transform.parent.gameObject.SetActive(true);
        specificPlayer.canClickThroughText = true;
        _hasPressedButton = false;
        gameText.text = textPrompt;
        yield return StartCoroutine(Delay(() => _hasPressedButton));
        specificPlayer.canClickThroughText = false;
        gameText.transform.parent.gameObject.SetActive(false);
    }
    private IEnumerator Delay(System.Func<bool> condition)
    {
        while (!condition())
        {
            yield return null;
        }
    }

    public void SetButtonTrue()
    {
        _hasPressedButton = true;
    }
}
