using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Pedestrian : MonoBehaviour
{
    private NavMeshAgent _agent;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetRoute(Vector3 destination)
    {
        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
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
        Debug.Log($"[Pedestrian] {gameObject.name} has been hit!");
        if (_agent != null) _agent.enabled = false;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        SphereCollider sc = gameObject.GetComponent<SphereCollider>() ?? gameObject.AddComponent<SphereCollider>();
        sc.radius = 0.5f;

        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
