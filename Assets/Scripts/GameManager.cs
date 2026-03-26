using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Minigames Refereneces")]
    [SerializeField]
    private Minigame1 _minigame1;
    [SerializeField]
    private Minigame2 _minigame2;
    //TODO: Minigame3

    private bool _inMinigame;
    
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameMusic();
            AudioManager.Instance.PitchRegular();
        }
    }

    public bool GetInMinigame()
    {
        return _inMinigame;
    }
    public void SetInMinigame(bool value)
    {
        _inMinigame = value;
    }

    public void StartMinigame1()
    {
        _inMinigame = true;
        _minigame1.StartMinigame();
    }
    
    public void StartMinigame2()
    {
        _inMinigame = true;
        //TODO: llamar a minigame2
    }
    
    public void StartMinigame3()
    {
        _inMinigame = true;
        //TODO: llamar a minigame3
    }
}
