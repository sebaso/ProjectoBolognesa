using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Minigames Refereneces")]
    [SerializeField]
    private Minigame1 _minigame1;
    [SerializeField]
    private Minigame2 _minigame2;
    [SerializeField]
    private Minigame3 _minigame3;

    //TODO: quitar los metodos de setminigame (estos son para pruebas)
    [SerializeField] private GameObject _prefabminigame2;
    [SerializeField] private GameObject _prefabminigame3;

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
        
        //TODO: quitar los metodos de setminigame (estos son para pruebas)
        SetMinigame1(1);
        SetMinigame2(true, _prefabminigame2);
        SetMinigame3(1, _prefabminigame3);
    }
    
    public bool GetInMinigame()
    {
        return _inMinigame;
    }
    public void SetInMinigame(bool value)
    {
        _inMinigame = value;
    }

    //TODO: llamar al llegar un client a la zona de inspection
    public void SetMinigame1(int value)
    {
        //1 para no borracho
        //2 para algo de alcohol pero puede entrar
        //3 para borracho
        _minigame1.SetWinState(value);
    }
    
    //TODO: llamar al llegar un client a la zona de inspection
    public void SetMinigame2(bool hasAnomaly, GameObject minigame2MonsterPrefab)
    {
        _minigame2.SetAnomaly(hasAnomaly);
        _minigame2.SetMonsterPrefab(minigame2MonsterPrefab);
    }
    
    //TODO: llamar al llegar un client a la zona de inspection
    public void SetMinigame3(int drugState, GameObject minigame3MonsterPrefab)
    {
        //0 no drogado
        //1 drogado
        _minigame3.SetState(drugState);
        _minigame3.SetMonsterPrefab(minigame3MonsterPrefab);
    }

    public void StartMinigame1()
    {
        _inMinigame = true;
        _minigame1.StartMinigame();
    }
    
    public void StartMinigame2()
    {
        _inMinigame = true;
        _minigame2.StartMinigame();
    }
    
    public void StartMinigame3()
    {
        _inMinigame = true;
        _minigame3.StartMinigame();
    }

    //TODO: DESABILITAR CURSOR CON PLAYERCAMERA.INSTANCE.ONDISABLE() AL SALIR DEL MINIJUEGO
}
