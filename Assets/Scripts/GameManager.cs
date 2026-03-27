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
        //SetMinigame1(1);
        //SetMinigame2(true, _prefabminigame2);
        //SetMinigame3(1, _prefabminigame3);
    }
    
    public bool GetInMinigame()
    {
        return _inMinigame;
    }
    public void SetInMinigame(bool value)
    {
        _inMinigame = value;
    }

    public void SetMinigame1()
    {
        //1 para no borracho
        //2 para algo de alcohol pero puede entrar
        //3 para borracho
        int value = 1;
        
        float clientSobriety = QueueManager.Instance.CurrentInspectingClient.sobriety;

        if ((clientSobriety * 100) <= 33)
            value = 1;
        else if ((clientSobriety * 100) <= 66)
            value = 2;
        else
            value = 3;
        
        _minigame1.SetWinState(value);
    }
    
    public void SetMinigame2()
    {
        bool hasAnomaly = QueueManager.Instance.CurrentInspectingClient.hasIllegalItems;

        GameObject minigame2MonsterPrefab = QueueManager.Instance.CurrentInspectingClient.minigame2MonsterPrefab;
        
        _minigame2.SetAnomaly(hasAnomaly);
        _minigame2.SetMonsterPrefab(minigame2MonsterPrefab);
    }
    
    public void SetMinigame3()
    {
        //0 no drogado
        //1 drogado

        int drugState = QueueManager.Instance.CurrentInspectingClient.pupils;
        
        GameObject minigame3MonsterPrefab = QueueManager.Instance.CurrentInspectingClient.minigame3MonsterPrefab;
        
        _minigame3.SetState(drugState);
        _minigame3.SetMonsterPrefab(minigame3MonsterPrefab);
    }

    public void StartMinigame1()
    {
        SetMinigame1();
        
        _inMinigame = true;
        _minigame1.StartMinigame();
    }
    
    public void StartMinigame2()
    {
        SetMinigame2();
        
        _inMinigame = true;
        _minigame2.StartMinigame();
    }
    
    public void StartMinigame3()
    {
        SetMinigame3();
        _inMinigame = true;
        _minigame3.StartMinigame();
    }

}
