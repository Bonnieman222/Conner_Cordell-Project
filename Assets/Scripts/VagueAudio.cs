using UnityEngine;
using UnityEngine.UI;

public class VolumeGroupSlider : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;

    [Header("Audio Targets")]
    public AudioSource[] audioSources;

    [Header("Save Settings")]
    public string playerPrefsKey = "VolumeGroup";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(playerPrefsKey, 1f);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        SetVolume(savedVolume);
    }

    public void SetVolume(float value)
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.volume = value;
        }

        PlayerPrefs.SetFloat(playerPrefsKey, value);
        PlayerPrefs.Save();
    }
}