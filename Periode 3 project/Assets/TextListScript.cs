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
    [SerializeField] private TMP_Text gameText;
    private bool _hasPressedButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        
    }

    public async Task ShowPrompts(List<string> textPrompts)
    {
        for (int i = 0; i < textPrompts.Count; i++)
        {
            _hasPressedButton = false;
            gameText.text = textPrompts[i];
            await Delay(() => _hasPressedButton);

        }
        gameText.text = "";
    }

    public async Task ShowPrompt(string textPrompt)
    {
        _hasPressedButton = false;
        gameText.text = textPrompt;
        await Delay(() => _hasPressedButton);
        gameText.text = "";
    }

    private async Task Delay(System.Func<bool> condition)
    {
        while (!condition())
        {
            await Task.Yield();
        }
    }

    public void SetButtonTrue()
    {
        _hasPressedButton = true;
    }
}
