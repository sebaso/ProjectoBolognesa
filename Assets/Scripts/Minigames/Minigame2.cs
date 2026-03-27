using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Minigame2 : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject _minigameCanvas;
    [SerializeField]
    private RectTransform _detectorParent;
    [SerializeField]
    private RectTransform _maskRevealer;
    [SerializeField]
    private RectTransform _skeletonMonster;
    [SerializeField]
    private Slider _timeSlider;
    [SerializeField]
    private RectTransform[] _anomaliesBoxes;
    [SerializeField]
    private RectTransform[] _normalObjBoxes;
    private RectTransform _anomaly;
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private GameObject _monsterParent;
    [SerializeField]
    private MonsterMinigame2 _monsterReferences;
    
    [Header("Minigame Settings")]
    [SerializeField]
    private float _timeLimit = 10f;
    [SerializeField]
    private bool _hasAnomaly;
    [SerializeField]
    private List<RectTransform> _bodyCheckpoints;
    [SerializeField]
    [Tooltip("Distancia a la que detecta el objeto")]
    [Range(1, 50)]
    private float _detectionRadius;
    
    [Header("Sprites")]
    [SerializeField]
    private Sprite[] _anomaliesSprites;
    [SerializeField]
    private Sprite[] _normalObjSprites;
    [SerializeField]
    private Sprite _noneSprite;
    [SerializeField]
    private Sprite _loseSprite;

    private float _currentTime;
    private bool _isGameActive;
    private GameObject _actualMonsterPrefab;
    private HashSet<RectTransform> _scannedCheckpoints = new HashSet<RectTransform>();

    private Vector2 _monsterNormalLocalPosition;

    [Header("Audio")]
    public AudioClip winSFX;
    public AudioClip loseSFX;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        _minigameCanvas.SetActive(false);
        //StartMinigame();
    }

    private void Update()
    {
        if (!_isGameActive) return;
        
        MoveToolToMouse();
        
        _currentTime -= Time.deltaTime;
        _timeSlider.value = _currentTime;
        
        if (_currentTime <= 0)
        {
            Lose();
            ShowResult(_loseSprite);
            return;
        }

        CheckScanningLogic();
    }

    private void LateUpdate()
    {
        if (!_isGameActive) return;

        _skeletonMonster.position = _detectorParent.position;

        // if (_hasAnomaly)
        // {
        //     float distance = Vector2.Distance(_maskRevealer.position, _anomaly.position);
        //
        //     if (distance <= _detectionRadius)
        //     {
        //         Win();
        //     }
        // }
    }

    //Esto se llamará desde el objeto que usa el player
    public void StartMinigame()
    {
        //ResetMinigame();
        
        Debug.Log("Starting Minigame!");
        SetMonsterReferences();
        SetObjects(_hasAnomaly);
        _isGameActive = true;
        
        _currentTime = _timeLimit;
        _timeSlider.maxValue = _timeLimit;
        _timeSlider.value = _timeLimit;

        _scannedCheckpoints.Clear();
        _resultImage.gameObject.SetActive(false);
        _maskRevealer.gameObject.SetActive(true);
        _minigameCanvas.gameObject.SetActive(true);
    }

    public void SetAnomaly(bool value)
    {
        _hasAnomaly = value;
    }

    public void SetMonsterPrefab(GameObject monsterPrefab)
    {
        _actualMonsterPrefab = Instantiate(monsterPrefab, _monsterParent.transform);
    }

    private void SetMonsterReferences()
    {
        _monsterReferences = _minigameCanvas.GetComponentInChildren<MonsterMinigame2>();

        if (_monsterReferences == null)
        {
            Debug.Log("No hay datos del mosntruo para los rayosX");
            return;
        }

        _detectorParent = _monsterReferences.GetDetectorParent();
        _maskRevealer = _monsterReferences.GetMaskRevealer();
        _skeletonMonster = _monsterReferences.GetSkeletonMonster();
        _anomaliesBoxes = _monsterReferences.GetAnomaliesBoxes();
        _normalObjBoxes =  _monsterReferences.GetNormalObjBoxes();
        _bodyCheckpoints = _monsterReferences.GetBodyCheckpoints();
    }

    private void ResetObjects()
    {
        for (int i = 0; i < _anomaliesBoxes.Length; i++)
        {
            _anomaliesBoxes[i].GetComponent<Image>().sprite = null;
            _anomaliesBoxes[i].gameObject.SetActive(false);
        }
        
        for (int i = 0; i < _normalObjBoxes.Length; i++)
        {
            _normalObjBoxes[i].GetComponent<Image>().sprite = null;
            _normalObjBoxes[i].gameObject.SetActive(false);
        }
    }

    private void ResetMinigame()
    {
        if(_actualMonsterPrefab != null)
            Destroy(_actualMonsterPrefab);
    }

    public void SetObjects(bool hasAnomaly)
    {
        ResetObjects();
        
        _hasAnomaly = hasAnomaly;

        if (hasAnomaly)
        {
            int rndmBox = Random.Range(1, _anomaliesBoxes.Length);
            
            _anomaly = _anomaliesBoxes[rndmBox];
            
            int rndmAnomaly = Random.Range(1, _anomaliesSprites.Length);
            
            _anomaly.GetComponent<Image>().sprite = _anomaliesSprites[rndmAnomaly];
            
            _anomaly.gameObject.SetActive(true);
        }
        
        int rndmCantNormalObj= Random.Range(0, 2);

        if (rndmCantNormalObj == 0) return;

        int rndmNormalBox = Random.Range(1, _normalObjBoxes.Length);
            
        int rndmNormalObj = Random.Range(1, _normalObjSprites.Length);
            
        _normalObjBoxes[rndmNormalBox].GetComponent<Image>().sprite = _normalObjSprites[rndmNormalObj];
            
        _normalObjBoxes[rndmNormalBox].gameObject.SetActive(true);
    }

    private void CheckScanningLogic()
    {
        if (_hasAnomaly)
        {
            float distance = Vector2.Distance(_maskRevealer.position, _anomaly.position);

            if (distance <= _detectionRadius)
            {
                Win();
                ShowResult(_anomaly.GetComponent<Image>().sprite);
            }
        }
        else
        {
            foreach (var checkpoint in _bodyCheckpoints)
            {
                if (!_scannedCheckpoints.Contains(checkpoint))
                {
                    float distance = Vector2.Distance(_maskRevealer.position, checkpoint.position);
                    if(distance <= _detectionRadius)
                        _scannedCheckpoints.Add(checkpoint);
                }
            }

            if (_scannedCheckpoints.Count >= _bodyCheckpoints.Count && _bodyCheckpoints.Count > 0)
            {
                Win();
                ShowResult(_noneSprite);
            }
        }
    }

    private void MoveToolToMouse()
    {
        Camera cam = _minigameCanvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _detectorParent, 
            Input.mousePosition,
            cam,
            out Vector2 localMousePosition);
        
        _maskRevealer.anchoredPosition = localMousePosition;
        //_skeletonMonster.anchoredPosition = -localMousePosition;
    }

    private void ShowResult(Sprite sprite)
    {
        _resultImage.sprite = sprite;
        _resultImage.gameObject.SetActive(true);
    }

    private void Win()
    {
        _isGameActive = false;
        
        InspectorSheet.Instance.RevealIllegalItems();
        Debug.Log("You win!");
        PlaySound(winSFX);
    }

    private void Lose()
    {
        _isGameActive = false;
        _maskRevealer.gameObject.SetActive(false);
        
        Debug.Log("You lose!");
        PlaySound(loseSFX);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    public void StopMinigame()
    {
        _isGameActive = false;
        
        _minigameCanvas.SetActive(false);
        
        ResetMinigame();
        PlayerCamera.Instance.OnDisableCursor();
        GameManager.Instance.SetInMinigame(false);
    }
}
