using UnityEngine;

public class BouncerManager : MonoBehaviour
{
    public static BouncerManager Instance { get; private set; }

    [Header("Bouncer Rules")]
    public int minimumAge = 18;
    public bool requireID = true;
    public float minSobriety = 0.7f;

    [Header("Inspection State")]
    public bool isMinigameActive = false;
    public string currentActiveTool = "";

    // Events that minigame scripts can listen to
    public event System.Action<Client, string> OnMinigameStarted;
    public event System.Action<string> OnMinigameCompleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {

    }

    public void StartInspectionTool(string toolName)
    {
        Client client = GetCurrentClient();
        if (client == null) return;

        if (isMinigameActive)
        {
            Debug.LogWarning($"[Bouncer] Already inspecting with {currentActiveTool}. Finish that first.");
            return;
        }

        isMinigameActive = true;
        currentActiveTool = toolName;
        Debug.Log($"[Bouncer] Started inspection minigame with tool: {toolName} on client: {client.gameObject.name}");

        OnMinigameStarted?.Invoke(client, toolName);
    }

    public void CompleteInspectionTool()
    {
        if (!isMinigameActive) return;

        Debug.Log($"[Bouncer] Completed inspection minigame with tool: {currentActiveTool}");
        OnMinigameCompleted?.Invoke(currentActiveTool);

        isMinigameActive = false;
        currentActiveTool = "";
    }

    public void AcceptClient()
    {
        if (isMinigameActive)
        {
            Debug.LogWarning("[Bouncer] Cannot make a decision while an inspection minigame is active!");
            return;
        }

        Client client = GetCurrentClient();
        if (client == null) return;

        bool isAllowed = CheckAttributes(client);
        if (isAllowed)
        {
            Debug.Log("<color=green>[Bouncer] Correct! Client admitted.</color>");
        }
        else
        {
            Debug.Log("<color=red>[Bouncer] MISTAKE! You let in someone who didn't meet requirements.</color>");
        }

        client.EnterRestaurant();
        QueueManager.Instance.RemoveClient(client);
    }

    public void RejectClient()
    {
        if (isMinigameActive)
        {
            Debug.LogWarning("[Bouncer] Cannot make a decision while an inspection minigame is active!");
            return;
        }

        Client client = GetCurrentClient();
        if (client == null) return;

        bool isAllowed = CheckAttributes(client);
        if (!isAllowed)
        {
            Debug.Log("<color=green>[Bouncer] Correct! You rejected them.</color>");
        }
        else
        {
            Debug.Log("<color=red>[Bouncer] MISTAKE! You rejected someone who was allowed.</color>");
        }

        client.LeaveRejected();
        QueueManager.Instance.RemoveClient(client);
    }

    private Client GetCurrentClient()
    {
        if (QueueManager.Instance != null && QueueManager.Instance.WaitingCount > 0)
        {
            return QueueManager.Instance._waitingClients[0];
        }
        return null;
    }

    private bool CheckAttributes(Client client)
    {
        if (client.age < minimumAge) return false;
        if (requireID && !client.hasID) return false;
        if (client.sobriety < minSobriety) return false;
        return true;
    }
}
