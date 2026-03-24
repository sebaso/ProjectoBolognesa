using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float _sensX = 0f;
    [SerializeField]
    private float _sensY = 0f;
    // Dirección del movimiento
    private float xRotation;
    private float yRotation;
    [SerializeField]
    private Transform _orientation;
    private Vector2 _lookInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = _lookInput.x * Time.deltaTime * _sensX;
        float mouseY = _lookInput.y * Time.deltaTime * _sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // YAW
        _orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // PITCH
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    public void SetLookInput(Vector2 lookInput)
    {
        _lookInput = lookInput;
    }
}
