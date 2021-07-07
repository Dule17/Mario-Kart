using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionsDropDown;
    Resolution[] resolutions;
    public Toggle fullScreen;
    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDropDown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex=0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add("" + resolutions[i].width + "x" + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width&& resolutions[i].height == Screen.currentResolution.height) currentResolutionIndex = i;
        }
        fullScreen.isOn = Screen.fullScreen;
        resolutionsDropDown.AddOptions(options);
        resolutionsDropDown.value = currentResolutionIndex;
        resolutionsDropDown.RefreshShownValue();
    }



    public void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height,Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullScreen(bool state)
    {
        Screen.fullScreen = state;
    }
}
