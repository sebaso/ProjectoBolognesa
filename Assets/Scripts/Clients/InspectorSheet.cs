using UnityEngine;
using TMPro;

public class InspectorSheet : MonoBehaviour
{
    public TextMeshPro nameText;
    public TextMeshPro ageText;
    public TextMeshPro alcoholLevelText;
    public TextMeshPro illegalItemText;
    public TextMeshPro pupilsText;
    public DoorOpener exitDoor;

    private Client _currentClient;
    private bool _isAlcoholRevealed;
    private bool _isItemsRevealed;
    private bool _isPupilsRevealed;
    private static InspectorSheet _instance;
    public static InspectorSheet Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
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
        ClearSheet();
    }

    void Update()
    {
        if (QueueManager.Instance == null) return;
        Client frontClient = QueueManager.Instance.CurrentInspectingClient;
        if (_currentClient != frontClient)
        {
            if (_currentClient != null)
            {
                _currentClient.OnReachedInspection -= UpdateSheet;
            }
            _currentClient = frontClient;

            if (_currentClient != null)
            {
                _isAlcoholRevealed = false;
                _isItemsRevealed = false;
                _isPupilsRevealed = false;
                _currentClient.OnReachedInspection += UpdateSheet;

                ClearSheet();
            }
            else
            {
                ClearSheet();
            }
        }
    }
    public void RevealAlcohol()
    {
        _isAlcoholRevealed = true;
        UpdateSheet();
    }

    public void RevealIllegalItems()
    {
        _isItemsRevealed = true;
        UpdateSheet();
    }

    public void RevealPupils()
    {
        _isPupilsRevealed = true;
        UpdateSheet();
    }
    public void AcceptClient()
    {
        if (_currentClient == null) return;
        _currentClient.EnterRestaurant();
        QueueManager.Instance.RemoveClient(_currentClient);
        ClearSheet();
    }

    public void RejectClient()
    {
        if (_currentClient == null) return;
        if (exitDoor != null) exitDoor.OpenDoor();
        _currentClient.LeaveRejected();
        QueueManager.Instance.RemoveClient(_currentClient);
        ClearSheet();
    }

    private void UpdateSheet()
    {
        if (_currentClient == null) return;
        if (nameText) nameText.text = $"Nombre: {_currentClient.clientName}";
        if (ageText) ageText.text = $"Edad: {_currentClient.age}";

        string sobrietyText = "";
        if ((_currentClient.sobriety * 100) <= 33)
            sobrietyText = "Bajo";
        else if ((_currentClient.sobriety * 100) <= 66)
            sobrietyText = "Medio";
        else
            sobrietyText = "Alto";
        
        if (alcoholLevelText)
            alcoholLevelText.text = _isAlcoholRevealed ? $"Alcohol: {sobrietyText}" : "Alcohol: ???";

        if (illegalItemText)
            illegalItemText.text = _isItemsRevealed ? $"Item Ilegal: {(_currentClient.hasIllegalItems ? "SI" : "NO")}" : "Item Ilegal: ???";

        if (pupilsText)
            pupilsText.text = _isPupilsRevealed ? $"Pupilas: {(_currentClient.pupils == 0 ? "Normal" : "Dilatadas")}" : "Pupilas: ???";

    }

    private void ClearSheet()
    {
        if (nameText) nameText.text = "Nombre: --";
        if (ageText) ageText.text = "Edad: --";
        if (alcoholLevelText) alcoholLevelText.text = "Alcohol: --";
        if (illegalItemText) illegalItemText.text = "Item Ilegal: --";
        if (pupilsText) pupilsText.text = "Pupilas: --";
    }

    void OnDestroy()
    {
        if (_currentClient != null)
        {
            _currentClient.OnReachedInspection -= UpdateSheet;
        }
    }
}
