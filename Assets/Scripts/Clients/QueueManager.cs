using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QueueManager : MonoBehaviour
{
    public static QueueManager Instance { get; private set; }

    public Transform queuePoint;
    public Transform inspectionPoint;
    public Transform entrancePoint;
    public Transform queueEntrance;
    public Transform exitPoint;
    public Vector3 queueDirection = Vector3.back;
    public float queueSpacing = 1.2f;
    public int maxQueueSize = 10;

    public List<Client> _waitingClients = new();

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

    public int WaitingCount => _waitingClients.Count;
    public bool IsQueueFull => _waitingClients.Count >= maxQueueSize;
    public Client CurrentInspectingClient => (_waitingClients.Count > 0) ? _waitingClients[0] : null;

    public void AddClient(Client client)
    {
        if (_waitingClients.Contains(client)) return;

        _waitingClients.Add(client);
        UpdateQueuePositions();
    }

    public void RemoveClient(Client client)
    {
        if (_waitingClients.Contains(client))
        {
            _waitingClients.Remove(client);
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < _waitingClients.Count; i++)
        {
            if (i == 0 && inspectionPoint != null)
            {
                _waitingClients[i].GoToInspectionSpot(inspectionPoint.position);
            }
            else
            {
                Vector3 slotPos = GetSlotPosition(i);
                _waitingClients[i].EnterWaitQueue(slotPos);
            }
        }
    }

    public Vector3 GetSlotPosition(int index)
    {
        Vector3 origin = queuePoint != null ? queuePoint.position : Vector3.zero;
        Vector3 direction = queueDirection == Vector3.zero ? Vector3.back : queueDirection.normalized;
        Vector3 candidate = origin + direction * (queueSpacing * index);
        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            return hit.position;

        return candidate;
    }
}