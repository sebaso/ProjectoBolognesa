using UnityEngine;
using System.Collections;

public class RayGun : Pickeable
{
    [Header("RayGun Settings")]
    public float range = 50f;
    public LayerMask targetLayer;
    public Transform firePoint;
    public LineRenderer laserEffect;
    public float laserDuration = 0.1f;

    private bool _isHeld = false;

    public override void Pick(Transform holdPoint)
    {
        base.Pick(holdPoint);
        _isHeld = true;
    }

    public override void Drop()
    {
        base.Drop();
        _isHeld = false;
    }

    void Update()
    {
        if (_isHeld && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("[RayGun] Pew Pew!");

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 direction = firePoint != null ? firePoint.forward : transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, targetLayer))
        {
            Client client = hit.collider.GetComponentInParent<Client>();
            if (client != null)
            {
                client.OnHit();
                Debug.Log($"[RayGun] Shot Client: {client.gameObject.name}");
            }

            Pedestrian ped = hit.collider.GetComponentInParent<Pedestrian>();
            if (ped != null)
            {
                ped.OnHit();
                Debug.Log($"[RayGun] Shot Pedestrian: {ped.gameObject.name}");
            }

            SneakyClient sneaky = hit.collider.GetComponentInParent<SneakyClient>();
            if (sneaky != null)
            {
                sneaky.OnHit();
                Debug.Log($"[RayGun] Shot SneakyClient: {sneaky.gameObject.name}");
            }

            if (laserEffect != null)
            {
                StartCoroutine(LaserVisual(origin, hit.point));
            }
        }
        else if (laserEffect != null)
        {
            StartCoroutine(LaserVisual(origin, origin + direction * range));
        }
    }

    private IEnumerator LaserVisual(Vector3 start, Vector3 end)
    {
        laserEffect.enabled = true;
        laserEffect.SetPosition(0, start);
        laserEffect.SetPosition(1, end);
        yield return new WaitForSeconds(laserDuration);
        laserEffect.enabled = false;
    }
}
