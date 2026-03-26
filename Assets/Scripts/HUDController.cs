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
    private bool _isPanelActive = false;
    public bool IsPanelActive => _isPanelActive;
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
        _nightPanelAnimator.SetTrigger("show");
    }
    public void OnEnableIntroPanel(int nightNumber, int numberClients)
    {
        _isPanelActive = true;
        PlayerCamera.Instance.OnEnableCursor();
        Time.timeScale = 0f;
        _introPanel.SetActive(true);
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
