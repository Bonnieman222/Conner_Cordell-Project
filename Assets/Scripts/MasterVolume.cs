using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeSlider : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;

    private const string VolumeKey = "MasterVolume";

    void Start()
    {
        // Load saved volume or default to full volume
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
}