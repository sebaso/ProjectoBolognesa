using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    private Image _crossHairOpen;
    [SerializeField]
    private Image _crossHairClosed;
    [SerializeField]
    private PlayerController _player;
    [Header("Night Panel")]
    [SerializeField]
    private ClientManager _clientManager;
    [SerializeField]
    private Animator _nightPanelAnimator;
    [SerializeField]
    private TMP_Text _nightNumberText;
    [Header("Intro Panel")]
    [SerializeField]
    private GameObject _introPanel;
    [SerializeField]
    private TextMeshProUGUI _numberNightTitle;
    [SerializeField]
    private TextMeshProUGUI _numberClients;
    [Header("EndNight Panel")]
    [SerializeField]
    private GameObject _endNightPanel;
    [SerializeField]
    private TextMeshProUGUI _clientsAttend;
    [SerializeField]
    private TextMeshProUGUI _clientsDennied;
    [SerializeField]
    private TextMeshProUGUI _sneakyClients;
    [SerializeField]
    private TextMeshProUGUI _totalPoints;
    [SerializeField]
    private Image _filledStartBar;
    [SerializeField]
    private GameObject _nextNightButton;
    [SerializeField]
    private GameObject _finishGameButton;
    [SerializeField]
    private TextMeshProUGUI _monstruosFaltantesValue;
    [SerializeField]
    private TextMeshProUGUI _puntosTotalesValue;
    [Header("FinishGame Panel")]
    [SerializeField]
    private GameObject _finishGamePanel;
    [SerializeField]
    private GameObject _winGameText;
    [SerializeField]
    private GameObject _looseGameText;
    private bool _isPanelActive = false;
    public bool IsPanelActive => _isPanelActive;
    private int _currentNightNumber;
    private int _numberClientsValue;
    private bool _nightFinished;
    private static HUDController _instance;
    public static HUDController Instance => _instance;
    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        _crossHairOpen.gameObject.SetActive(true);
    }
    void Update()
    {
        calculateTextsUI();
    }
    void OnEnable()
    {
        _player.OnHoldTool += UpdateCrossHair;
        _clientManager.OnNightStart += ShowStartNightPanel;
        _clientManager.OnNightEnds += ShowNightFinishedPanel;
    }

    void OnDisable()
    {
        _player.OnHoldTool -= UpdateCrossHair;
        _clientManager.OnNightStart -= ShowStartNightPanel;
        _clientManager.OnNightEnds -= ShowNightFinishedPanel;
    }
    private void calculateTextsUI()
    {
        _monstruosFaltantesValue.text =  ClientManager.Instance.ClientsToProcess.ToString();
        _puntosTotalesValue.text = Mathf.Clamp(CurrencyController.Instance.TotalScore, 0, CurrencyController.Instance.MaxScore).ToString();
    }
    private void UpdateCrossHair(bool grabTool)
    {
        if (grabTool)
        {
            _crossHairOpen.gameObject.SetActive(false);
            _crossHairClosed.gameObject.SetActive(true);
        }
        else
        {
            _crossHairClosed.gameObject.SetActive(false);
            _crossHairOpen.gameObject.SetActive(true);
        }
    }
    public void ShowStartNightPanel(int nightNumber)
    {
        _nightNumberText.text = nightNumber.ToString();
        _nightPanelAnimator.SetTrigger("show");
    }

    public void ShowNightFinishedPanel()
    {
        _nightNumberText.text = "Finished";
        _nightFinished = true;
        _nightPanelAnimator.SetTrigger("show");
    }
    public void OnEnableIntroPanel(int nightNumber, int numberClients)
    {
        _isPanelActive = true;
        PlayerCamera.Instance.OnEnableCursor();
        Time.timeScale = 0f;
        _introPanel.SetActive(true);
        _currentNightNumber = nightNumber;
        _numberClientsValue = numberClients;
        _numberNightTitle.text = "Noche " + nightNumber.ToString();
        _numberClients.text = numberClients.ToString() + " Monstruos";

    }
    private void OnDisableIntroPanel()
    {
        _isPanelActive = false;
        PlayerCamera.Instance.OnDisableCursor();
        Time.timeScale = 1f;
        _introPanel.SetActive(false);
    }
    public void OnFinishNight()
    {
        if(!_nightFinished) return;
        _isPanelActive = true;
        PlayerCamera.Instance.OnEnableCursor();
        Time.timeScale = 0f;
        _endNightPanel.SetActive(true);
        int totalPoints = CurrencyController.Instance.TotalScore;
        int maxPoints = CurrencyController.Instance.MaxScore;
        _clientsAttend.text = ClientManager.Instance.CorrectClientsAcepted.ToString();
        _clientsDennied.text = ClientManager.Instance.CorrectClientsRejected.ToString();
        _sneakyClients.text = ClientManager.Instance.IntrudersAcepted.ToString();
        _totalPoints.text = totalPoints.ToString();
        float starsAmmount = maxPoints > 0 ? (float)totalPoints / maxPoints : 0f;
        starsAmmount = Mathf.Clamp01(starsAmmount);
        _filledStartBar.fillAmount = starsAmmount;
        if(starsAmmount == 1 || starsAmmount == 0)
        {
            _nextNightButton.SetActive(false);
            _finishGameButton.SetActive(true);
        }
        else
        {
            _nextNightButton.SetActive(true);
            _finishGameButton.SetActive(false);
        }
    }

    public void OnNextNight()
    {
        _isPanelActive = false;
        _nightFinished = false;
        PlayerCamera.Instance.OnDisableCursor();
        Time.timeScale = 1f;
        _endNightPanel.SetActive(false);
        int nextNightNumber = _currentNightNumber + 1;
        int nextNumberClients = nextNightNumber * 5;
        OnEnableIntroPanel(nextNightNumber, nextNumberClients);
    }
    public void OnFinishGameFinal()
    {
        _endNightPanel.SetActive(false);
        _finishGamePanel.SetActive(true);
        int totalPoints = CurrencyController.Instance.TotalScore;
        int maxPoints = CurrencyController.Instance.MaxScore;
        float starsAmmount = maxPoints > 0 ? (float)totalPoints / maxPoints : 0f;
        starsAmmount = Mathf.Clamp01(starsAmmount);
        if(starsAmmount == 1)
        {
            _winGameText.SetActive(true);
            _looseGameText.SetActive(false);
        }
        else
        {
            _winGameText.SetActive(false);
            _looseGameText.SetActive(true);
        }
    }
    public void OnStartNewNight()
    {
        OnDisableIntroPanel();
        _clientManager.StartNewNight();
    }
    public void OnFinishGame()
    {
        OnDisableIntroPanel();
    }
}
