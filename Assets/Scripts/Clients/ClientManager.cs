using UnityEngine;

public class ClientManager : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject clientPrefab;
    public Transform spawnPoint;
    public Transform queuePoint;
    public float spawnInterval = 3f;

    private float _spawnTimer;

    void Start()
    {
        _spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (QueueManager.Instance != null && QueueManager.Instance.IsQueueFull)
        {
            return;
        }

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnClient();
            _spawnTimer = spawnInterval;
        }
    }

    private void SpawnClient()
    {
        if (clientPrefab == null || spawnPoint == null || queuePoint == null)
        {
            Debug.LogWarning("[ClientManager] Missing spawning references!");
            return;
        }

        GameObject clientObj = Instantiate(clientPrefab, spawnPoint.position, spawnPoint.rotation);
        Client client = clientObj.GetComponent<Client>();
        if (client != null)
        {
            client.SetQueuePoint(queuePoint);
            QueueManager.Instance?.AddClient(client);
        }
    }
}
