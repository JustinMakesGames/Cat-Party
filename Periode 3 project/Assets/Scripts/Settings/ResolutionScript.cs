using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResolutionScript : MonoBehaviour
{
    public TMP_Dropdown resDropDown;
    public Toggle fullScreenToggle;

    private Resolution[] _allResolutions;
    private bool _isFullScreen;
    private int _selectedResolution;

    private List<Resolution> selectedResolutionList = new List<Resolution>();
    private void Start()
    {
        _isFullScreen = true;
        _allResolutions = Screen.resolutions;

        string newRes;
        List<string> resolutionStringList = new List<string>();

        foreach (Resolution res in _allResolutions)
        {
            newRes = res.width.ToString() + "x" + res.height.ToString();

            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes.ToString());
                selectedResolutionList.Add(res);
            }
            
        }

        resDropDown.AddOptions(resolutionStringList);
    }

    public void ChangeResolution()
    {
        _selectedResolution = resDropDown.value;
        Screen.SetResolution(selectedResolutionList[_selectedResolution].width, selectedResolutionList[_selectedResolution].height, _isFullScreen);
    }

    public void ChangeFullScreen()
    {
        _isFullScreen = fullScreenToggle.isOn;
        Screen.SetResolution(selectedResolutionList[_selectedResolution].width, selectedResolutionList[_selectedResolution].height, _isFullScreen);

    }

}
