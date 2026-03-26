using UnityEngine;
using System.Collections;

public class Broomstick : Pickeable
{
    [Header("Swing Settings")]
    public float swingDuration = 0.15f;
    public float swingAngle = 60f;
    public Transform hitPoint;
    public float hitRadius = 1.5f;
    public LayerMask targetLayer;

    private bool _isSwinging = false;
    private bool _isHeld = false;
    private Quaternion _defaultLocalRotation;

    public override void Pick(Transform holdPoint)
    {
        base.Pick(holdPoint);
        _isHeld = true;
        _defaultLocalRotation = transform.localRotation;
    }

    public override void Drop()
    {
        base.Drop();
        _isHeld = false;
    }

    void Update()
    {
        if (_isHeld && Input.GetMouseButtonDown(0) && !_isSwinging)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    private IEnumerator SwingCoroutine()
    {
        _isSwinging = true;

        Quaternion startRot = _defaultLocalRotation;
        // como somos pobres y no tenemos animador, animamos por script en eje X :)
        Quaternion endRot = startRot * Quaternion.Euler(swingAngle, 0, 0);
        float elapsed = 0;
        while (elapsed < swingDuration)
        {
            transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / swingDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = endRot;
        CheckHit();
        elapsed = 0;
        while (elapsed < swingDuration)
        {
            transform.localRotation = Quaternion.Slerp(endRot, startRot, elapsed / swingDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = startRot;

        _isSwinging = false;
    }

    private void CheckHit()
    {
        Vector3 checkPos = hitPoint != null ? hitPoint.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(checkPos, hitRadius, targetLayer);
        foreach (var hit in hits)
        {
            SneakyClient sneaky = hit.GetComponentInParent<SneakyClient>();
            if (sneaky != null)
            {
                sneaky.OnHit();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 checkPos = hitPoint != null ? hitPoint.position : transform.position;
        Gizmos.DrawWireSphere(checkPos, hitRadius);
    }
}
