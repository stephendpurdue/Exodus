using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    private Resolution[] resolutions;
    private bool isInitializing = false;

    void Start()
    {
        isInitializing = true; // Lock event listeners
        InitializeSettings();
        isInitializing = false; // Unlock event listeners
    }

    private void InitializeSettings()
    {
        // 1. Gather all unique refresh rates for unique resolutions to avoid duplicate dropdown entries
        resolutions = Screen.resolutions
            .Select(r => new { r.width, r.height })
            .Distinct()
            .Select(r => new Resolution { width = r.width, height = r.height })
            .ToArray();

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentSystemResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Best guess for default system resolution
            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentSystemResIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // 2. Fetch or create default player prefs
        int savedResIndex = PlayerPrefs.GetInt("ResolutionPreference", currentSystemResIndex);
        int savedQualityIndex = PlayerPrefs.GetInt("QualityPreference", QualitySettings.GetQualityLevel());
        bool savedFullscreen = PlayerPrefs.GetInt("FullscreenPreference", Screen.fullScreen ? 1 : 0) == 1;
        float savedVolume = PlayerPrefs.GetFloat("VolumePreference", 0f);

        // Security clamp to prevent crashes on foreign monitors
        if (resolutions.Length > 0)
        {
            savedResIndex = Mathf.Clamp(savedResIndex, 0, resolutions.Length - 1);
            Resolution savedRes = resolutions[savedResIndex];

            // Unity quirk: Must pass refresh rate in SetResolution for it to stick in builds
            Screen.SetResolution(savedRes.width, savedRes.height, savedFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, Screen.currentResolution.refreshRateRatio);

            if (resolutionDropdown != null)
            {
                // Unhook listener, set value, rehook
                resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
                resolutionDropdown.value = savedResIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
        }

        if (QualitySettings.names.Length > 0)
        {
            savedQualityIndex = Mathf.Clamp(savedQualityIndex, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(savedQualityIndex);

            if (qualityDropdown != null)
            {
                qualityDropdown.onValueChanged.RemoveListener(SetQuality);
                qualityDropdown.value = savedQualityIndex;
                qualityDropdown.RefreshShownValue();
                qualityDropdown.onValueChanged.AddListener(SetQuality);
            }
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
            fullscreenToggle.isOn = savedFullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);

            StartCoroutine(SetVolumeDelayed(savedVolume));
        }
    }

    private IEnumerator SetVolumeDelayed(float volume)
    {
        yield return new WaitForEndOfFrame();
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Volume", volume);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        if (isInitializing) return;

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, Screen.currentResolution.refreshRateRatio);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        PlayerPrefs.Save();
        Debug.Log($"Set Resolution: {resolution.width}x{resolution.height}");
    }

    public void SetVolume(float volume)
    {
        if (isInitializing) return;

        if (audioMixer != null) audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("VolumePreference", volume);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        if (isInitializing) return;

        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityPreference", qualityIndex);
        PlayerPrefs.Save();
        Debug.Log($"Set Quality: {qualityIndex}");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isInitializing) return;

        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Set Fullscreen: {isFullscreen}");
    }
}
