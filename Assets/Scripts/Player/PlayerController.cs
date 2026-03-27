using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Action<bool> OnHoldTool;
    [Header("Camera Settings")]
    private Camera _camera;
    public bool showGizmos = false;
    [Header("Collision pre detection")]
    [SerializeField]
    private LayerMask _checkLayer;
    [SerializeField]
    private Transform _checkPoint;
    [SerializeField]
    private float _checkSize = 0.3f;
    [Header("Physics")]
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Transform _groundCheckPoint;
    [SerializeField]
    private Vector3 _groundCheckSize;
    private bool _grounded;
    [Header("Player controlers")]
    private Vector2 _lookInput;
    private bool _usingGamepad = false;
    [Header("Aiming")]
    [SerializeField]
    private float _interactionDistance = 1000f;
    [SerializeField]
    private float _camRayLenght;
    [SerializeField]
    private LayerMask _pointerLayer;
    [SerializeField]
    private Transform _aimingPivot;
    [Header("Inventory")]
    [SerializeField]
    private Transform _holdingPoint;
    private IPickeable _holdingTool;
    private GameObject _holdingToolGO;
    private String _toolTypeTag;
    private LayerMask _toolLayer;
    private LayerMask _tableLayer;
    [SerializeField]
    private LayerMask _clientLayer;
    void Awake()
    {
        _tableLayer = LayerMask.NameToLayer("Table");
        _toolLayer = LayerMask.NameToLayer("Tool");
        _clientLayer = LayerMask.NameToLayer("Client");
    }
    void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        GroundCheck();
    }
    #region Methods
    /// <summary>
    /// Métodos nuevos del system input actualizado
    /// </summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        if (_lookInput.magnitude < 0.5f)
            _lookInput = Vector2.zero;
        _camera.GetComponent<PlayerCamera>().SetLookInput(_lookInput);
        _usingGamepad = context.control.device is Gamepad;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && !GameManager.Instance.GetInMinigame())
        {
            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _interactionDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }

                if (_holdingTool != null)
                {
                    if (hit.collider.gameObject.layer == _clientLayer)
                    {
                        switch (CheckHoldedTool())
                        {
                            case 1:
                                if (hit.collider.gameObject.CompareTag("Client"))
                                    GameManager.Instance.StartMinigame1();
                                break;
                            case 2:
                                if (hit.collider.gameObject.CompareTag("Client"))
                                    GameManager.Instance.StartMinigame2();
                                break;
                            case 3:
                                if (hit.collider.gameObject.CompareTag("Client"))
                                    GameManager.Instance.StartMinigame3();
                                break;
                            case 4:
                                if (hit.collider.gameObject.CompareTag("Sneaky"))
                                    if(!_holdingToolGO.GetComponent<Broomstick>().IsSwinging)
                                        StartCoroutine(_holdingToolGO.GetComponent<Broomstick>().SwingCoroutine());
                                break;
                            case 5:
                                //TODO: interactuar con la pistola
                                break;
                            default:
                                break;
                        }
                    }
                    else if (hit.collider.gameObject.layer == _tableLayer || hit.collider.gameObject.layer == _toolLayer)
                    {
                        DropTool();
                        return;
                    }
                }


                if (_holdingTool == null)
                {
                    IPickeable pickeable = hit.collider.GetComponent<IPickeable>();
                    if (pickeable != null)
                    {
                        _toolTypeTag = hit.collider.gameObject.tag;
                        _holdingToolGO = hit.collider.gameObject;
                        _holdingToolGO.GetComponent<BoxCollider>().enabled = false;
                        _holdingTool = pickeable;
                        pickeable.Pick(_holdingPoint);
                        OnHoldTool?.Invoke(true);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Comprueba si está en contacto con el suelo y actualiza la booleana _grounded.
    /// </summary>
    private void GroundCheck()
    {
        // Solo vamos a comprobar si es mayor que 0, así que no necesitamos más capacidad en el buffered
        Collider[] colliderBuffer = new Collider[1];
        // Comprobamos si hay contacto con el suelo. Lo hacemos mediante un OverlapBoxNonAlloc,
        // para no consumir más memoria de la necesaria, ya que esta función la vamos a hacer
        // de manera continuada.
        Physics.OverlapBoxNonAlloc(_groundCheckPoint.position,
                                    _groundCheckSize / 2f,
                                    colliderBuffer,
                                    transform.rotation,
                                    _groundLayer);
        // Actualizamos el estado de _grounded según el buffer
        _grounded = colliderBuffer[0] != null;
    }
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        // Cambiamos el color del Gizmo
        Gizmos.color = Color.blue;
        // Mostramos el Gizmo del check de colisión frontal
        Gizmos.DrawWireSphere(_checkPoint.position, _checkSize);

        // Cambiamos el color del Gizmo
        Gizmos.color = Color.red;
        // Mostramos el Gizmo del ground check pintando un cubo
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }

    private int CheckHoldedTool()
    {
        switch (_toolTypeTag)
        {
            case "Alcoholimetro":
                return 1;
            case "RayosX":
                return 2;
            case "Lupa":
                return 3;
            case "Porra":
                return 4;
            case "Raygun":
                return 5;
            default:
                return -1;
        }
    }

    private void PickTool()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _interactionDistance))
        {
            IPickeable pickeable = hit.collider.GetComponent<IPickeable>();

            if (pickeable != null)
            {
                _toolTypeTag = hit.collider.gameObject.tag;
                _holdingToolGO = hit.collider.gameObject;
                _holdingToolGO.GetComponent<BoxCollider>().enabled = false;
                _holdingTool = pickeable;
                pickeable.Pick(_holdingPoint);
                OnHoldTool?.Invoke(true);
            }
        }
    }
    private void DropTool()
    {
        _holdingToolGO.GetComponent<BoxCollider>().enabled = true;
        _holdingTool.Drop();
        _holdingTool = null;
        OnHoldTool?.Invoke(false);
    }
    #endregion
}
