using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Client : MonoBehaviour
{
    public GameObject[] clientModels;
    public enum State
    {
        WalkingToQueue,     // Heading from spawn to restaurant queue point
        Waiting,            // Queued at entrance — no table available yet
        WalkingToTable,     // Told where to sit, navigating to seat
        WaitingForFood,     // Seated, patience ticking down
        Eating,             // Food arrived — eating timer running
        Leaving,            // Walking to exit, will be destroyed on arrival
        Angry,               // Patience ran out — leaving unhappy
        Inspecting,         // At the bouncer's check window
        Admitted,           // Allowed into the restaurant
        FinalExit           // Walking past the door before despawning
    }

    public State CurrentState { get; private set; } = State.Waiting;
    public float maxPatience = 60f;
    public float eatDuration = 8f;
    public string clientName;
    public int age;
    public bool hasID;
    public float sobriety;
    public int pupils;
    public bool hasIllegalItems;
    public string dressCode;
    public GameObject minigame2MonsterPrefab;
    public GameObject minigame3MonsterPrefab;
    public float destinationTolerance = 0.5f;
    public bool _minigame1Played;
    public bool _minigame2Played;
    public bool _minigame3Played;

    public int money;
    public int happiness;
    public int nationality;
    private float _patience;
    private Transform _seatPoint;
    private Transform _queuePoint;
    private NavMeshAgent _agent;
    private Vector3 _queueSlotPosition;
    private Quaternion _targetRotation;
    private bool Initialized = false;
    [SerializeField]
    private Animator _animator;
    private bool _hasNotifiedArrival = false;

    public event System.Action OnReachedInspection;

    public float PatienceRatio => _patience / maxPatience;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _patience = maxPatience;
    }

    public void SetQueuePoint(Transform queuePoint)
    {
        _queuePoint = queuePoint;
    }
    void Update()
    {
        if (!Initialized)
        {
            initialize();
        }
        switch (CurrentState)
        {
            case State.WalkingToQueue:
                if (HasReachedDestination())
                    ArriveAtQueue();
                break;

            case State.Waiting:
                if (HasReachedDestination())
                {
                    Freeze();
                    transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * 5f);
                }
                break;

            case State.WalkingToTable:
                if (HasReachedDestination())
                    SitDown();
                break;

            case State.WaitingForFood:
                TickPatience();
                break;

            case State.Leaving:
            case State.Angry:
                if (InspectorSheet.Instance != null && InspectorSheet.Instance.exitDoor != null)
                {
                    if (!InspectorSheet.Instance.exitDoor.IsFullyOpen)
                    {
                        Freeze();
                        return;
                    }
                }

                if (!_agent.hasPath || _agent.isStopped)
                {
                    WalkToExit();
                }

                if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance < Mathf.Max(_agent.stoppingDistance, destinationTolerance))
                {
                    Vector3 forwardDir = transform.forward;
                    if (_agent.velocity.sqrMagnitude > 0.1f) forwardDir = _agent.velocity.normalized;
                    WalkTo(transform.position + forwardDir * 3f);
                    SetState(State.FinalExit);
                }
                break;

            case State.Admitted:
                if (_agent.hasPath && !_agent.pathPending && _agent.remainingDistance < Mathf.Max(_agent.stoppingDistance, destinationTolerance))
                {
                    Vector3 forwardDir = transform.forward;
                    if (_agent.velocity.sqrMagnitude > 0.1f) forwardDir = _agent.velocity.normalized;
                    WalkTo(transform.position + forwardDir * 3f);
                    SetState(State.FinalExit);
                }
                break;
            case State.FinalExit:
                if (HasReachedDestination())
                    Destroy(gameObject);
                break;

            case State.Inspecting:
                if (HasReachedDestination())
                {
                    Freeze();
                    if (QueueManager.Instance != null && QueueManager.Instance.inspectionPoint != null)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, QueueManager.Instance.inspectionPoint.rotation, Time.deltaTime * 5f);
                    }

                    if (!_hasNotifiedArrival)
                    {
                        _hasNotifiedArrival = true;
                        OnReachedInspection?.Invoke();
                        Debug.Log($"[Client] {gameObject.name} arrived at inspection point.");
                    }
                }
                break;
        }
        UpdateAnimation();
    }

    public void ArriveAtQueue()
    {
        if (QueueManager.Instance != null)
        {
            QueueManager.Instance.AddClient(this);
            Debug.Log("[Client] Arrived at entrance, joining queue line.");
        }
    }
    public void initialize()
    {
        int randomIndex = Random.Range(0, clientModels.Length);
        GameObject selectedModel = Instantiate(clientModels[randomIndex], transform.position, clientModels[randomIndex].transform.rotation);
        selectedModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        selectedModel.transform.SetParent(transform);
        Initialized = true;
        selectedModel.transform.position = selectedModel.transform.position + new Vector3(0, -0.5f, 0);

        _animator = selectedModel.GetComponent<Animator>();
        age = Random.Range(15, 45);
        clientName = GetRandomName();
        hasID = Random.value > 0.1f; // 90% tiene ID
        sobriety = Random.Range(0.0f, 1.0f);
        pupils = Random.Range(0, 1);
        hasIllegalItems = Random.value > 0.9f;
        string[] styles = { "Casual", "Formal", "Deportivo" };
        dressCode = styles[Random.Range(0, styles.Length)];
        minigame2MonsterPrefab = selectedModel.GetComponent<MonsterPrefabManager>().GetMinigame2Prefab();
        minigame3MonsterPrefab =  selectedModel.GetComponent<MonsterPrefabManager>().GetMinigame3Prefab();
    }

    private string GetRandomName()
    {
        string[] monsterNames = {
            "Chupacabras", "El Silbón", "La Llorona", "Trasgu", "Gamusino",
            "Cadejo", "Ojáncanu", "Meiga", "Duende", "Güije",
            "El Coco", "La Cuca", "Cuélebre", "Lamia", "Brujo",
            "Espectro", "Siniestro", "Zarpa", "Colmillo", "Gorguer","El Pepe Arrozero" ,"El Pepe Bolognesas"
        };
        string[] monsterTitles = {
            "El Temible", "Del Pantano", "Sombrío", "El Horrible", "Sangriento",
            "El Pestilente", "De la Oscuridad", "Acechador", "Gruñón", "El Baboso",
            "De las Sombras", "Pálido", "Gritón", "El Infame", "Maldito"
        };
        return monsterNames[Random.Range(0, monsterNames.Length)] + " " + monsterTitles[Random.Range(0, monsterTitles.Length)];
    }
    public void BeginJourney()
    {
        if (QueueManager.Instance != null && QueueManager.Instance.queueEntrance != null)
        {
            SetState(State.WalkingToQueue);
            WalkTo(QueueManager.Instance.queueEntrance.position);
        }
        else
        {
            ArriveAtQueue();
        }
    }
    public int getAge()
    {
        return age;
    }
    public bool GetHasID()
    {
        return hasID;
    }
    public float GetSobriety()
    {
        return sobriety;
    }
    public int GetPupils()
    {
        return pupils;
    }
    public bool GetHasIllegalItems()
    {
        return hasIllegalItems;
    }
    public string GetDressCode()
    {
        return dressCode;
    }

    public void EnterWaitQueue(Vector3 slotPosition)
    {
        _queueSlotPosition = slotPosition;

        if (QueueManager.Instance != null)
        {
            Vector3 lookDir = -QueueManager.Instance.queueDirection.normalized;
            if (lookDir == Vector3.zero) lookDir = Vector3.forward;
            float variance = Random.Range(-5f, 5f); // para que no miren todos perfectamente alineados
            _targetRotation = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, variance, 0);
        }
        SetState(State.Waiting);
        WalkTo(slotPosition);
    }

    public void MoveToQueueSlot(Vector3 newSlotPosition)
    {
        _queueSlotPosition = newSlotPosition;
        WalkTo(newSlotPosition);
    }

    public void GoToInspectionSpot(Vector3 pos)
    {
        _queueSlotPosition = pos;
        SetState(State.Inspecting);
        WalkTo(pos);
    }


    private void SitDown()
    {
        Freeze();
        transform.position = _seatPoint.position;

        _patience = maxPatience;
        SetState(State.WaitingForFood);
        Debug.Log($"[Client] Seated. Patience: {maxPatience}s");
    }

    private void TickPatience()
    {
        _patience -= Time.deltaTime;
        if (_patience <= 0f)
        {
            _patience = 0f;
            LeaveAngry();
        }
    }

    public void ReceiveFood()
    {
        if (CurrentState != State.WaitingForFood) return;

        SetState(State.Eating);
        Debug.Log("[Client] Food received! Eating...");
        StartCoroutine(EatCoroutine());
    }

    private IEnumerator EatCoroutine()
    {
        yield return new WaitForSeconds(eatDuration);
        LeaveHappy();
    }

    public void EnterRestaurant()
    {
        SetState(State.Admitted);
        if (QueueManager.Instance != null && QueueManager.Instance.entrancePoint != null)
        {
            if (CheckParameters())
            {
                ClientManager.Instance.OnClientAccepted(true);
            }
            WalkTo(QueueManager.Instance.entrancePoint.position);
        }
        else
        {
            WalkTo(transform.position + transform.forward * 10f);
        }
    }

    public void LeaveRejected()
    {
        SetState(State.Angry);
        if (CheckParameters())
        {
            ClientManager.Instance.OnCorrectClientRejected();
        }
        WalkToExit();
        Debug.Log("[Client] Rejected! Leaving angry.");
    }
    private bool CheckParameters()
    {
        var _currentInspectingClient = QueueManager.Instance.CurrentInspectingClient;
        if (_currentInspectingClient.age >= 18)
        {
            Debug.Log("Es mayor de edad");
            if (_currentInspectingClient.sobriety <= 66)
            {
                Debug.Log("Esta sobrio");
                if (!_currentInspectingClient.hasIllegalItems)
                {
                    Debug.Log("No tiene objetos ilegales");
                    if (_currentInspectingClient.pupils == 0)
                    {
                        Debug.Log("No está drogado");
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void LeaveHappy()
    {
        happiness += 10;
        Debug.Log("[Client] Finished eating. Leaving happy.");
        StartLeaving();
    }

    private void LeaveAngry()
    {
        happiness -= 10;
        Debug.Log("[Client] Patience ran out! Leaving angry.");

        SetState(State.Angry);
        WalkToExit();
    }

    private void StartLeaving()
    {

        SetState(State.Leaving);
        WalkToExit();
    }

    private void WalkToExit()
    {
        Vector3 exitPos = transform.position + Vector3.back * 10f;

        if (QueueManager.Instance != null && QueueManager.Instance.exitPoint != null)
        {
            exitPos = QueueManager.Instance.exitPoint.position;
        }
        else if (_queuePoint != null)
        {
            // Fallback: move to queue point only if exitPoint is not defined
            exitPos = _queuePoint.position;
        }

        WalkTo(exitPos);
    }

    private void WalkTo(Vector3 destination)
    {
        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }
    }

    public void SetSpeed(float speed)
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        _agent.speed = speed;
    }

    public void OnHit()
    {
        if (_agent != null) _agent.enabled = false;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        SphereCollider sc = gameObject.GetComponent<SphereCollider>();
        if (sc == null) sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = 0.5f;

        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }
    //para que no se mueva como idiota
    private void Freeze()
    {
        _agent.ResetPath();
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
    }

    private bool HasReachedDestination()
    {
        if(_agent.enabled == false ) return false;
        if (_agent.pathPending) return false;

        float effectiveStoppingDistance = Mathf.Max(_agent.stoppingDistance, destinationTolerance);
        if (_agent.remainingDistance > effectiveStoppingDistance) return false;

        if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return false;
        return true;
    }
    private void UpdateAnimation()
    {
        if (_animator == null) return;

        float speed = _agent.velocity.magnitude;

        bool isMoving = speed > 0.1f;

        _animator.SetBool("Walking", isMoving);

        // opcional: correr
        _animator.SetBool("Running", speed > 3.0f);
    }
    private void SetState(State newState)
    {
        CurrentState = newState;
    }
    void OnDestroy()
    {
        if (QueueManager.Instance != null)
        {
            QueueManager.Instance.RemoveClient(this);
            ClientManager.Instance.clients.Remove(gameObject);
        }
    }

}