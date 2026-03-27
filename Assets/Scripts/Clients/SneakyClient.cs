using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SneakyClient : MonoBehaviour
{
    private enum SneakyState { Sneaking, Dissimulating, Reacted }

    [Header("Movement Settings")]
    public float slowSpeed = 1.5f;
    public float fastSpeed = 6.0f;

    [Header("Sneaky Settings")]
    public int maxSpotsBeforeReacting = 3;
    public float dissimulationCooldown = 2.0f;
    public float fleeDistance = 20f;

    private NavMeshAgent _agent;
    private SneakyState _state = SneakyState.Sneaking;
    private int _spotCount = 0;
    private bool _isFleeing = false;
    private float _cooldownTimer = 0f;
    private bool _wasSeenLastFrame = false;
    private Transform _entrancePoint;
    private Vector3 _spawnPos;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = slowSpeed;
        _spawnPos = transform.position;
    }

    void Start()
    {
        if (QueueManager.Instance != null)
            _entrancePoint = QueueManager.Instance.entrancePoint;

        if (_entrancePoint != null)
            _agent.SetDestination(_entrancePoint.position);
    }

    void Update()
    {
        switch (_state)
        {
            case SneakyState.Sneaking: UpdateSneaking(); break;
            case SneakyState.Dissimulating: UpdateDissimulating(); break;
            case SneakyState.Reacted: CheckReachedDestination(); break;
        }
    }

    private void UpdateSneaking()
    {
        bool isSeen = IsInView();

        if (isSeen)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;

            if (!_wasSeenLastFrame)
            {
                _spotCount++;
                _cooldownTimer = dissimulationCooldown;
                Debug.Log($"[SneakyClient] Spotted! Count: {_spotCount}");

                if (_spotCount >= maxSpotsBeforeReacting)
                    React();
                else
                {
                    _state = SneakyState.Dissimulating;
                    Debug.Log("[SneakyClient] Dissimulating...");
                }
            }
        }
        else
        {
            _agent.isStopped = false;
            _agent.speed = slowSpeed;
        }

        _wasSeenLastFrame = isSeen;
        CheckReachedDestination();
    }

    private void UpdateDissimulating()
    {
        if (IsInView())
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _cooldownTimer = dissimulationCooldown;
            return;
        }

        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer <= 0f)
        {
            _state = SneakyState.Sneaking;
            _wasSeenLastFrame = false;
            _agent.isStopped = false;
            _agent.speed = slowSpeed;
            if (_entrancePoint != null)
                _agent.SetDestination(_entrancePoint.position);
            //Debug.Log("[SneakyClient] Resuming sneaking...");
        }
        CheckReachedDestination();
    }
    private void React()
    {
        _state = SneakyState.Reacted;
        _agent.isStopped = false;

        if (Random.value > 0.5f)
            StartFleeing();
        else
            StartRunningForIt();
    }

    private void StartFleeing()
    {
        _isFleeing = true;
        _agent.speed = fastSpeed;

        if (NavMesh.SamplePosition(_spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
        else
            _agent.SetDestination(_spawnPos);

        //Debug.Log("[SneakyClient] PANIC! Fleeing to spawn...");
    }

    private void StartRunningForIt()
    {
        _agent.speed = fastSpeed;
        if (_entrancePoint != null)
            _agent.SetDestination(_entrancePoint.position);
        //Debug.Log("[SneakyClient] MAKE A RUN FOR IT!");
    }
    private bool IsInView()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        bool inViewport = viewportPos.z > 0
            && viewportPos.x > 0 && viewportPos.x < 1
            && viewportPos.y > 0 && viewportPos.y < 1;

        if (!inViewport) return false;
        Vector3 targetPos = transform.position + Vector3.up * 1.5f;
        Vector3 dir = targetPos - cam.transform.position;

        if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, dir.magnitude + 0.5f))
            return hit.transform == transform || hit.transform.IsChildOf(transform);

        return true;
    }
    public void OnHit()
    {
        Debug.Log("[SneakyClient] Ouch! I've been hit!");
        _agent.enabled = false;

        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<SphereCollider>();
        gameObject.GetComponent<SphereCollider>().radius = 0.5f;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }
    private void CheckReachedDestination()
    {
        if (_agent.pathPending) return;
        if (_agent.remainingDistance > _agent.stoppingDistance) return;
        if (_agent.hasPath && _agent.velocity.sqrMagnitude > 0.01f) return;

        Debug.Log(_isFleeing ? "[SneakyClient] Escaped!" : "[SneakyClient] Reached entrance!");
        ClientManager.Instance.OnSneakySneaked();
        Destroy(gameObject);
    }
}
