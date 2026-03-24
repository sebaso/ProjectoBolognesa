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
        Admitted            // Allowed into the restaurant
    }

    public State CurrentState { get; private set; } = State.Waiting;
    public float maxPatience = 60f;
    public float eatDuration = 8f;
    public int age;
    public bool hasID;
    public float sobriety;
    public string dressCode;

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
            case State.Admitted:
                if (HasReachedDestination())
                    Destroy(gameObject);
                break;
        }
    }

    public void ArriveAtQueue()
    {
        if (QueueManager.Instance != null)
        {
            QueueManager.Instance.AddClient(this);
        }
    }
    public void initialize()
    {
        //int randomIndex = Random.Range(0, clientModels.Length); cuando hayan modelos
        GameObject selectedModel = Instantiate(clientModels[0], transform.position, Quaternion.Euler(0, 0, 0));
        //selectedModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        selectedModel.transform.SetParent(transform);
        Initialized = true;
        selectedModel.transform.position = selectedModel.transform.position + new Vector3(0, -0.5f, 0);

        // Randomize Bouncer Attributes
        age = Random.Range(15, 45);
        hasID = Random.value > 0.1f; // 90% have ID
        sobriety = Random.Range(0.5f, 1.0f);
        string[] styles = { "Casual", "Formal", "Sporty" };
        dressCode = styles[Random.Range(0, styles.Length)];
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
            WalkTo(QueueManager.Instance.entrancePoint.position);
        }
        else
        {
            WalkTo(transform.position + transform.forward * 10f);
        }

        Debug.Log("[Client] Admitted to restaurant!");
    }

    public void LeaveRejected()
    {
        SetState(State.Angry);
        WalkToExit();
        Debug.Log("[Client] Rejected! Leaving angry.");
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
        Vector3 exitPos = _queuePoint != null ? _queuePoint.position : transform.position + Vector3.back * 10f;
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
    //para que no se mueva como idiota
    private void Freeze()
    {
        _agent.ResetPath();
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
    }

    private bool HasReachedDestination()
    {
        if (_agent.pathPending) return false;
        if (_agent.remainingDistance > _agent.stoppingDistance) return false;
        if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return false;
        return true;
    }

    private void SetState(State newState)
    {
        CurrentState = newState;
    }

}