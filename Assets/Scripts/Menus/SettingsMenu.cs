using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    // Populate the resolution dropdown menu with the available screen resolutions
    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // Load saved preferences or default to current screen/system settings
        int savedResIndex = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
        resolutionDropdown.value = savedResIndex;
        resolutionDropdown.RefreshShownValue();

        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityPreference", QualitySettings.GetQualityLevel()));
        Screen.fullScreen = PlayerPrefs.GetInt("FullscreenPreference", Screen.fullScreen ? 1 : 0) == 1;

        if (PlayerPrefs.HasKey("VolumePreference"))
        {
            audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("VolumePreference"));
        }
    }

    // Set the screen resolution based on the index of the dropdown menu
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        PlayerPrefs.Save();
    }

    // Set the volume of the audio mixer based on the value of the slider
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("VolumePreference", volume);
        PlayerPrefs.Save();
    }

    // Set the quality level based on the index of the dropdown menu
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityPreference", qualityIndex);
        PlayerPrefs.Save();
    }

    // Set the fullscreen mode based on the value of the toggle
    public void SetFullscreen(bool isfullscreen)
    {
        Screen.fullScreen = isfullscreen;
        PlayerPrefs.SetInt("FullscreenPreference", isfullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
