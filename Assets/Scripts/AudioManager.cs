using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip menuClip;
    public AudioClip gameClip;
    public AudioMixer mixer;
    [SerializeField] 
    private AudioMixerSnapshot normalSnapshot;
    [SerializeField] 
    private AudioMixerSnapshot mutedSnapshot;
    [Header("Fade Settings")]
    // Tiempo de fundido de la música
    [Range(1f, 3f)]
    public float fadeTime = 2f;
    [Range(0f,2f)]
    public float pitchTime = 1f;
    // Valor mínimo del pitch para cambiar el sonido 
    // al mostrar el menú de fin de partida.
    public float pitchSlow = 0.6f;

    private Coroutine fadeCoroutine;
    private Coroutine pitchCoroutine;

    // Constantes del PlayerPref
    private const string GeneralVPref = "GeneralVol";
    private const string MusicVPref = "MusicVol";
    private const string SFXVPref = "SFXVol";
    private const string MutePref = "MutedVolume";

    // Constantes del Mixer
    private const string GeneralVParam = "GeneralVolume";
    private const string MusicVParam = "MusicVolume";
    private const string SFXVParam   = "SFXVolume";

    private static AudioManager _instance;
    public static AudioManager Instance => _instance; 

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            // Método que indica un GameObject que debe ser conservado
            // al descargar la escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
       assingVolumeParameters();
    }

    private void assingVolumeParameters()
    {
        // --- General ---
        float generalVolume = PlayerPrefs.GetFloat(GeneralVPref, 0.5f);
        mixer.SetFloat(GeneralVParam, Mathf.Log10(generalVolume) * 20f);

        // --- Música ---
        float musicVolume = PlayerPrefs.GetFloat(MusicVPref, 0.5f);
        mixer.SetFloat(MusicVParam, Mathf.Log10(musicVolume) * 20f);

        // --- SFX ---
        float sfxVolume = PlayerPrefs.GetFloat(SFXVPref, 0.5f);
        mixer.SetFloat(SFXVParam, Mathf.Log10(sfxVolume) * 20f);

        // --- Mute ---
        bool muted = PlayerPrefs.GetInt(MutePref, 0) == 1;
        if (muted)
            mutedSnapshot.TransitionTo(0.1f);
        else
            normalSnapshot.TransitionTo(0.1f);
    }

    //  ----------------------------------------------MÚSICA----------------------------------------------------
    /// <summary>
    /// Reproduce la música del menú principal
    /// </summary>
    public void PlayMainMenuMusic()
    {
        if(audioSource.clip == menuClip) return;
        
        if(fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        // Iniciamos la corrutina
        fadeCoroutine = StartCoroutine(FadeAndChangeClip(menuClip));
    }

    public void PlayGameMusic()
    {
        if(audioSource.clip == gameClip) return;

        if(fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        // Iniciamos la corrutina
        fadeCoroutine = StartCoroutine(FadeAndChangeClip(gameClip));
    }

    private IEnumerator FadeAndChangeClip( AudioClip clip)
    {
        // Usamos la mitad del tiempo indicado porque
        // tenermos que hacer la salida y entrada
        float half = fadeTime / 2f;
        // Volumen actual modificado en opciones
        float actualVolume = PlayerPrefs.GetFloat(MusicVPref, 0.5f);
        //actualVolume = Mathf.Pow(10, actualVolume / 20f);

        // Fade Out
        float counter = half;
        while ( counter > 0f)
        {
            mixer.SetFloat(MusicVParam, Mathf.Log10(counter / half * actualVolume) * 20f);
            counter -= Time.deltaTime;
            yield return null;
        }
        // Cambiamos el clip
        audioSource.clip = clip;
        // Reproducimos el clip nuevo
        audioSource.Play();

        //Fade In
        counter = 0f;
        while ( counter < half)
        {
            mixer.SetFloat(MusicVParam, Mathf.Log10(counter / half * actualVolume) * 20f);
            counter += Time.deltaTime;
            yield return null;
        }
        mixer.SetFloat(MusicVParam, Mathf.Log10(actualVolume) * 20f);
    }

    //  ----------------------------------PITCH---------------------------------------------------------
    [ContextMenu("Pitch Slow")]
    public void PitchSlow()
    {
        if(pitchCoroutine != null) StopCoroutine(pitchCoroutine);
        pitchCoroutine = StartCoroutine(PitchChange(true));
    }

    [ContextMenu("Pitch Regular")]
    public void PitchRegular()
    {
        if(pitchCoroutine != null) StopCoroutine(pitchCoroutine);
        pitchCoroutine = StartCoroutine(PitchChange(false));
    }

    private IEnumerator PitchChange(bool slow)
    {
        float target = slow ? pitchSlow : 1f;
        float initial = audioSource.pitch;
        float count = 0f;

        while( count < pitchTime)
        {
            audioSource.pitch = Mathf.Lerp(initial , target, count / pitchTime);
            count += Time.deltaTime;
            yield return null;
        }
    }
 //  ---------------------------------------Options-Volume---------------------------------------------------

    public void SetGeneralVolume(float volume)
    {
        if ( volume <= 0)
        {
            mixer.SetFloat(GeneralVParam, -80f);
            PlayerPrefs.SetFloat(GeneralVPref, 0);   
        }
        else
        {
            mixer.SetFloat(GeneralVParam, Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(GeneralVPref, volume); 
        }
    }
    public float GetGeneralVolume()
    {
        float volume;
        mixer.GetFloat(GeneralVParam, out volume);
        return Mathf.Pow(10, volume / 20f);
    }
    public void SetMusicVolume(float volume)
    {
        if ( volume <= 0)
        {
            mixer.SetFloat(MusicVParam, -80f);
            PlayerPrefs.SetFloat(MusicVPref, 0);   
        }
        else
        {
            mixer.SetFloat(MusicVParam, Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(MusicVPref, volume); 
        }
    }
    public float GetMusicVolume()
    {
        float volume;
        mixer.GetFloat(MusicVParam, out volume);
        return Mathf.Pow(10, volume / 20f);
    }

    public void SetSFXVolume(float volume)
    {
        if ( volume <= 0)
        {
            mixer.SetFloat(SFXVParam, -80f);
            PlayerPrefs.SetFloat(SFXVPref, 0);   
        }
        else
        {
            mixer.SetFloat(SFXVParam, Mathf.Log10(volume) * 20f);
            PlayerPrefs.SetFloat(SFXVPref, volume); 
        }
    }
    public float GetSFXVolume()
    {
        float volume;
        mixer.GetFloat(SFXVParam, out volume);
        return Mathf.Pow(10, volume / 20f);
    }

public void SetMute(bool mute)
{
    if (mute)
        mutedSnapshot.TransitionTo(0.1f);
    else
        normalSnapshot.TransitionTo(0.1f);

    PlayerPrefs.SetInt(MutePref, mute ? 1 : 0);
}
    public bool GetMute()
    {
        if (PlayerPrefs.GetInt(MutePref) == 1)
            return true;
        else
            return false;
    }
}