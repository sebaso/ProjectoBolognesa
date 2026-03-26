using UnityEngine;

public class skyboxParallax : MonoBehaviour
{
    [SerializeField]
    private float _rotationParallax = 1f;
    void Update()
    {
        float yaw = Camera.main.transform.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);

        transform.rotation = Quaternion.Slerp(Quaternion.identity, targetRotation, _rotationParallax);
    }
}
