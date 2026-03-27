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
