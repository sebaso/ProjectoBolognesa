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
    private bool _usingGamepad;
    [Header("Aiming")] 
    [SerializeField]
    private float _interactionDistance = 3f;
    [SerializeField]
    private float _camRayLenght;
    [SerializeField]
    private LayerMask _pointerLayer;
    [SerializeField]
    private Transform _aimingPivot;
    [Header("Inventory")]
    [SerializeField]
    private Transform _holdingPoint;
    [SerializeField]
    private IPickeable _holdingTool;
    void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        GroundCheck();
        Debug.Log("Esta en el suelo:" + _grounded);
    }
    #region Methods
    /// <summary>
    /// Métodos nuevos del system input actualizado
    /// </summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        if(_lookInput.magnitude < 0.5f) 
            _lookInput = Vector2.zero;
        _camera.GetComponent<PlayerCamera>().SetLookInput(_lookInput);
        _usingGamepad = context.control.device is Gamepad;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_holdingTool != null)
            {
                //TODO: comprobar si se está soltando en la mesa o no (si no se está en la mesa no se suelta)
                //if(mesa)
                //DropItem();
                //else
                switch (CheckHoldedTool())
                {
                    case 1:
                        GameManager.Instance.StartMinigame1();
                        break;
                    case 2:
                        GameManager.Instance.StartMinigame2();
                        break;
                    case 3:
                        GameManager.Instance.StartMinigame3();
                        break;
                    case 4:
                        //TODO: interactuar con la pistola
                        break;
                    case 5:
                        //TODO: interactuar con la escoba
                        break;
                    default:
                        //No hacemos nada
                        break;
                }
            }
            TryInteract();
            
            //TODO: Detectar qué objeto tiene
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
        if(!showGizmos) return;
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
        //TODO: comprobar el tipo de herramienta
        //Según el tipo devuelve:
        //1: alcoholimetro
        //2: rayosx
        //3: lector pupila
        //4: pistola
        //5: escoba

        return 1;
    }

    private void TryInteract()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, _interactionDistance))
        {
            IPickeable pickeable = hit.collider.GetComponent<IPickeable>();

            if(pickeable != null)
            {
                _holdingTool = pickeable;
                pickeable.Pick(_holdingPoint);
                OnHoldTool?.Invoke(true);
            }
        }
    }
    private void DropItem()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, _interactionDistance))
        {
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * _interactionDistance );
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Table") || hit.collider.gameObject.CompareTag("Tool"))
            {
                if (_holdingTool != null)
                {
                    _holdingTool.Drop();
                    _holdingTool = null;
                    OnHoldTool?.Invoke(false);
                }
            }
        }
    }
    #endregion
}
