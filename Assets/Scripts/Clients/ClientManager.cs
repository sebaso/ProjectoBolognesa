using UnityEngine;
using System.Collections.Generic;

public class ClientManager : MonoBehaviour
{
    public System.Action<int> OnNightStart;
    public System.Action OnNightEnds;
    [Header("Client Spawning")]
    public GameObject clientPrefab;
    public Transform[] spawnPoints;
    public Transform queuePoint;
    public float clientSpawnInterval = 10f;
    public int maxClients = 10;
    public List<GameObject> clients;
    private float _clientTimer;
    [SerializeField]
    private int _nightClients;
    [SerializeField]
    private int _remaningClients;
    [Header("Pedestrian Spawning")]
    public GameObject pedestrianPrefab;
    public float pedSpawnIntervalMin = 2f;
    public float pedSpawnIntervalMax = 8f;
    public int minBlobSize = 1;
    public int maxBlobSize = 4;
    public List<PedestrianRoute> pedestrianRoutes;
    private float _pedestrianTimer;

    [Header("Sneaky Spawning")]
    public Transform sneakySpawnPoints;
    public GameObject sneakyClientPrefab;
    public float sneakySpawnInterval = 20f;
    [Range(0, 1)] public float sneakySpawnChance = 0.3f;
    private float _sneakyTimer;
    private int _nightClientNumberMultiplier = 10;
    private int _currentNight;

    [System.Serializable]
    public class PedestrianRoute
    {
        public Transform[] starts;
        public Transform end;
    }

    public static ClientManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _clientTimer = clientSpawnInterval;
        _pedestrianTimer = Random.Range(pedSpawnIntervalMin, pedSpawnIntervalMax);
        _sneakyTimer = sneakySpawnInterval;
        int nextNight = _currentNight + 1;
        int nextClients = nextNight * _nightClientNumberMultiplier;
        HUDController.Instance.OnEnableIntroPanel(nextNight, nextClients);
    }

    public void StartNewNight()
    {
       _currentNight++;
       _nightClients = _currentNight * _nightClientNumberMultiplier;
       _remaningClients = _nightClients;
       OnNightStart?.Invoke(_currentNight);
    }
    void Update()
    {
        if(_nightClients > 0)
        {
            _clientTimer -= Time.deltaTime;
            if (_clientTimer <= 0f)
            {
                TrySpawnClient();
                _clientTimer = clientSpawnInterval;
            }
        }
        _pedestrianTimer -= Time.deltaTime;
        if (_pedestrianTimer <= 0f)
        {
            SpawnPedestrianBlob();
            _pedestrianTimer = Random.Range(pedSpawnIntervalMin, pedSpawnIntervalMax);
        }
        _sneakyTimer -= Time.deltaTime;
        if (_sneakyTimer <= 0f)
        {
            if (Random.value < sneakySpawnChance)
            {
                SpawnSneakyClient();
            }
            _sneakyTimer = sneakySpawnInterval;
        }
    }

    private void TrySpawnClient()
    {
        if (QueueManager.Instance != null && QueueManager.Instance.IsQueueFull) return;
        if (clients.Count >= maxClients) return;

        SpawnClient();
    }

    private void SpawnClient()
    {
        if (clientPrefab == null || spawnPoints == null || spawnPoints.Length == 0 || queuePoint == null)
        {
            Debug.LogWarning("[ClientManager] Missing spawning references!");
            return;
        }

        Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject clientObj = Instantiate(clientPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
        Client client = clientObj.GetComponent<Client>();
        if (client != null)
        {
            client.SetQueuePoint(queuePoint);
            client.BeginJourney();
        }
        clients.Add(clientObj);
        _nightClients--;
    }

    private void SpawnPedestrianBlob()
    {
        if (pedestrianPrefab == null || pedestrianRoutes.Count == 0) return;

        int blobSize = Random.Range(minBlobSize, maxBlobSize + 1);
        PedestrianRoute route = pedestrianRoutes[Random.Range(0, pedestrianRoutes.Count)];

        if (route.starts == null || route.starts.Length == 0 || route.end == null) return;

        for (int i = 0; i < blobSize; i++)
        {
            Transform selectedStart = route.starts[Random.Range(0, route.starts.Length)];
            Vector3 offset = new(Random.Range(-0.8f, 0.8f), 0, Random.Range(-0.8f, 0.8f));
            GameObject pedObj = Instantiate(pedestrianPrefab, selectedStart.position + offset, selectedStart.rotation);
            Pedestrian ped = pedObj.GetComponent<Pedestrian>();
            if (ped != null)
            {
                ped.SetRoute(route.end.position);
            }
        }
    }

    private void SpawnSneakyClient()
    {
        if (sneakyClientPrefab == null || sneakySpawnPoints == null) return;

        Instantiate(sneakyClientPrefab, sneakySpawnPoints.position, sneakySpawnPoints.rotation);
        Debug.Log("[ClientManager] A sneaky client has appeared!");
    }

    public void OnClientReachedDestination()
    {
        _remaningClients--;
        if(_remaningClients <= 0)
            OnNightEnds?.Invoke();
    }
}
