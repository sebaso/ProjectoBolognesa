using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Minigame3 : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject _hudMinigameCanvas;
    [SerializeField]
    private RectTransform _backgroundArea;
    [SerializeField]
    private RectTransform _scannerTool;
    [SerializeField] 
    private RectTransform _pupil;
    [SerializeField]
    private Slider _progressBar;
    [SerializeField]
    private Canvas _MinigameCanvas;
    [SerializeField]
    private GameObject _monsterParent;
    [SerializeField]
    private MonsterMinigame3 _monsterReferences;
    
    [Header("Eye Settings")]
    [SerializeField]
    private float _pupilSpeed = 2f;
    [SerializeField]
    private float _erraticAmount = 3f;
    
    [Header("Progress Settings")]
    [SerializeField]
    private float _catchRadius = 75f;
    [SerializeField]
    private float _fillRate = 0.2f;
    [SerializeField]
    private float _drainRate = 0.1f;
    [SerializeField]
    private float _timeLimit = 10f;
    
    [Header("Result Settings")]
    [SerializeField]
    private Image _resultImage;
    [SerializeField]
    private Sprite _win0Sprite;
    [SerializeField]
    private Sprite _win1Sprite;
    [SerializeField]
    private Sprite _loseSprite;

    private int _drugState;
    private float _currentTime;
    private float _currentProgress;
    private bool _hasStarted;
    private bool _inGame;
    private GameObject _actualMonsterPrefab;

    private float _noiseSeedX;
    private float _noiseSeedY;
    
    void Start()
    {
        _hudMinigameCanvas.SetActive(false);
        //SetState(1);
        //StartMinigame();
    }

    void Update()
    {
        if (!_inGame) return;

        if (_currentTime >= _timeLimit)
        {
            Lose();
            return;
        }
        
        _currentTime += Time.deltaTime;

        MoveScannerToMouse();
        MovePupilErratically();
        CheckScanningProgress();
    }

    public void SetState(int state)
    {
        _drugState =  state;
    }
    
    public void SetMonsterPrefab(GameObject monsterPrefab)
    {
        _actualMonsterPrefab = Instantiate(monsterPrefab, _monsterParent.transform);
    }

    public void SetMonsterReferences()
    {
        _monsterReferences = _MinigameCanvas.GetComponentInChildren<MonsterMinigame3>();

        if (_monsterReferences == null)
        {
            Debug.Log("No hay datos del onstruo para el escaner de retina");
            return;
        }

        _backgroundArea = _monsterReferences.GetBackGroundArea();
        _pupil =  _monsterReferences.GetPupil();
    }
    public void StartMinigame()
    {
        //ResetMinigame();
        Debug.Log("Starting Minigame");
        SetMonsterReferences();
        _resultImage.gameObject.SetActive(false);
        _hudMinigameCanvas.SetActive(true);
        _noiseSeedX = Random.Range(0f, 100f);
        _noiseSeedY = Random.Range(100f, 200f);
        
        _progressBar.minValue = 0f;
        _progressBar.maxValue = 1f;
        _progressBar.value = 0f;
        _currentProgress = 0f;

        _currentTime = 0f;

        _inGame = true;
    }

    private void ResetMinigame()
    {
        Debug.Log("ResetMinigame");
        if(_actualMonsterPrefab != null)
            Destroy(_actualMonsterPrefab);
    }

    private void MoveScannerToMouse()
    {
        Camera cam = _MinigameCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _backgroundArea,
            Input.mousePosition,
            cam,
            out Vector2 localMousePosition);
        
        float limitX = (_backgroundArea.rect.width / 2f) - (_scannerTool.rect.width / 2);
        float limitY = (_backgroundArea.rect.height / 2f) - (_scannerTool.rect.height / 2);
        
        localMousePosition.x = Mathf.Clamp(localMousePosition.x, -limitX, limitX);
        localMousePosition.y = Mathf.Clamp(localMousePosition.y, -limitY, limitY);
        
        _scannerTool.anchoredPosition = localMousePosition;
    }

    private void MovePupilErratically()
    {
        float noiseX = Mathf.PerlinNoise(Time.time * _pupilSpeed, _noiseSeedX);
        float noiseY = Mathf.PerlinNoise(_noiseSeedY, Time.time * _pupilSpeed);
        
        float limitX = (_backgroundArea.rect.width / 2f) - (_pupil.rect.width / 2);
        float limitY = (_backgroundArea.rect.height / 2f) - (_pupil.rect.height / 2);
        
        float targetX = Mathf.Lerp(-limitX, limitX, noiseX);
        float targetY = Mathf.Lerp(-limitY, limitY, noiseY);
        
        Vector2 currentPosition = _pupil.anchoredPosition;
        Vector2 targetPosition = new Vector2(targetX, targetY);
        
        _pupil.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, Time.deltaTime * _erraticAmount);
    }

    private void CheckScanningProgress()
    {
        float distance = Vector2.Distance(_scannerTool.anchoredPosition, _pupil.anchoredPosition);
        
        if(distance <= _catchRadius)
            _currentProgress += _fillRate * Time.deltaTime;
        else
            _currentProgress -= _drainRate * Time.deltaTime;

        _currentProgress = Mathf.Clamp01(_currentProgress);
        _progressBar.value = _currentProgress;

        if (_currentProgress >= 1f)
            Win();
    }

    private void Win()
    {
        _inGame = false;
        if(_drugState == 0)
            ShowResult(_win0Sprite);
        else
            ShowResult(_win1Sprite);
        
        InspectorSheet.Instance.RevealPupils();
        Debug.Log("You Win!");
    }

    private void Lose()
    {
        _inGame = false;
        ShowResult(_loseSprite);
        Debug.Log("You Lose!");
    }

    private void ShowResult(Sprite sprite)
    {
        _resultImage.gameObject.SetActive(true);
        _resultImage.sprite = sprite;
    }

    public void StopMinigame()
    {
        if (_inGame) return;
        _hudMinigameCanvas.SetActive(false);
        _hasStarted = false;
        ResetMinigame();
        PlayerCamera.Instance.OnDisableCursor();
        GameManager.Instance.SetInMinigame(false);
    }
}
