using UnityEngine;
using System;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float masterVolume = 0.8f;
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.85f;

    [Header("Graphics Settings")]
    public bool isFullscreen = true;
    public int resolutionIndex = 0;
    public int qualityLevel = 2; // 0: Low, 1: Medium, 2: High, 3: Ultra
    public bool vSyncEnabled = true;

    [Header("Controls Settings")]
    [Range(0.1f, 3.0f)] public float mouseSensitivity = 1.0f;
    public bool invertYAxis = false;

    // Events
    public static event Action OnSettingsChanged;

    private const string KEY_MASTER_VOL = "Settings_MasterVol";
    private const string KEY_BGM_VOL = "Settings_BgmVol";
    private const string KEY_SFX_VOL = "Settings_SfxVol";
    private const string KEY_FULLSCREEN = "Settings_Fullscreen";
    private const string KEY_RES_INDEX = "Settings_ResIndex";
    private const string KEY_QUALITY = "Settings_Quality";
    private const string KEY_VSYNC = "Settings_VSync";
    private const string KEY_SENSITIVITY = "Settings_Sensitivity";
    private const string KEY_INVERT_Y = "Settings_InvertY";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOL, 0.8f);
        bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOL, 0.7f);
        sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOL, 0.85f);

        isFullscreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, Screen.fullScreen ? 1 : 0) == 1;
        resolutionIndex = PlayerPrefs.GetInt(KEY_RES_INDEX, 0);
        qualityLevel = PlayerPrefs.GetInt(KEY_QUALITY, QualitySettings.GetQualityLevel());
        vSyncEnabled = PlayerPrefs.GetInt(KEY_VSYNC, 1) == 1;

        mouseSensitivity = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 1.0f);
        invertYAxis = PlayerPrefs.GetInt(KEY_INVERT_Y, 0) == 1;

        ApplyAudioSettings();
        ApplyGraphicsSettings();
        ApplyControlsSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(KEY_MASTER_VOL, masterVolume);
        PlayerPrefs.SetFloat(KEY_BGM_VOL, bgmVolume);
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sfxVolume);

        PlayerPrefs.SetInt(KEY_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(KEY_RES_INDEX, resolutionIndex);
        PlayerPrefs.SetInt(KEY_QUALITY, qualityLevel);
        PlayerPrefs.SetInt(KEY_VSYNC, vSyncEnabled ? 1 : 0);

        PlayerPrefs.SetFloat(KEY_SENSITIVITY, mouseSensitivity);
        PlayerPrefs.SetInt(KEY_INVERT_Y, invertYAxis ? 1 : 0);

        PlayerPrefs.Save();
        OnSettingsChanged?.Invoke();
    }

    public void SetMasterVolume(float val)
    {
        masterVolume = Mathf.Clamp01(val);
        ApplyAudioSettings();
        SaveSettings();
    }

    public void SetBgmVolume(float val)
    {
        bgmVolume = Mathf.Clamp01(val);
        ApplyAudioSettings();
        SaveSettings();
    }

    public void SetSfxVolume(float val)
    {
        sfxVolume = Mathf.Clamp01(val);
        ApplyAudioSettings();
        SaveSettings();
    }

    public void SetFullscreen(bool val)
    {
        isFullscreen = val;
        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    public void SetQualityLevel(int index)
    {
        qualityLevel = Mathf.Clamp(index, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(qualityLevel, true);
        SaveSettings();
    }

    public void SetVSync(bool val)
    {
        vSyncEnabled = val;
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
        SaveSettings();
    }

    public void SetSensitivity(float val)
    {
        mouseSensitivity = Mathf.Clamp(val, 0.1f, 3.0f);
        SaveSettings();
    }

    public void SetInvertY(bool val)
    {
        invertYAxis = val;
        SaveSettings();
    }

    public void ApplyAudioSettings()
    {
        AudioListener.volume = masterVolume;
    }

    public void ApplyGraphicsSettings()
    {
        Screen.fullScreen = isFullscreen;
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
    }

    public void ApplyControlsSettings()
    {
        // Broadcasts to active controllers if needed
    }
}
