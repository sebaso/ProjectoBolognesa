using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField]
    private Slider generalSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Toggle muteCheck;

    private const string GeneralVPref = "GeneralVol";
    private const string MusicVPref = "MusicVol";
    private const string SFXVPref = "SFXVol";
    private const string MutePref = "MutedVolume";

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(GeneralVPref))
            generalSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(GeneralVPref)
            );
        if (PlayerPrefs.HasKey(MusicVPref))
            musicSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(MusicVPref)
            );
        if (PlayerPrefs.HasKey(SFXVPref))
            sfxSlider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(SFXVPref)
            );
        if (PlayerPrefs.HasKey(MutePref))
            muteCheck.SetIsOnWithoutNotify(
                PlayerPrefs.GetInt(MutePref) == 1
            );
    }
    public void OnGeneralVolumeChanged(float value)
    {
        AudioManager.Instance.SetGeneralVolume(value);
    }
    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }
    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
    public void OnMuteCheckChanged()
    {
        if(AudioManager.Instance.GetMute() == true)
            AudioManager.Instance.SetMute(false);
        else
            AudioManager.Instance.SetMute(true);
    }
}
