using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Camera _camera;
    private float _horizontal = 0f;
    private float _vertical = 0f;
    // Dirección del movimiento
    private Vector3 _direction;
    public bool showGizmos = false;

    // Velocidad deseada en base a la dirección y la velocidad máxima
    private Vector3 _desiredVelocity;
    [Header("Player Movement")]
    [SerializeField]
    private float _movementSpeed = 8f;
    [SerializeField]
    private float _acceleration = 30f;

    [Header("Collision pre detection")]
    [SerializeField]
    private LayerMask _checkLayer;
    [SerializeField]
    private Transform _checkPoint;
    [SerializeField]
    private float _checkSize = 0.3f;
    [Range(0,3)]
    [SerializeField]
    private float _checkDistance = 2f;
    private bool _isWalled;
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
    private float _camRayLenght;
    [SerializeField]
    private LayerMask _pointerLayer;
    [SerializeField]
    private Transform _aimingPivot;
    [SerializeField]
    private bool _isAiming=false;

    void Start()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        GroundCheck();
        CollisionPreDetection();
        Movement();
        Aiming();
    }
    #region Methods
    /// <summary>
    /// Métodos nuevos del system input actualizado
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _horizontal = input.x;
        _vertical = input.y;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        if(_lookInput.magnitude < 0.5f) 
            _lookInput = Vector2.zero;
        _usingGamepad = context.control.device is Gamepad;
    }
    private void Movement()
    {
        // Componemos el vector de dirección deseado a partir del input
        _direction.Set(_horizontal, 0f, _vertical);
        // Para asegurarnos de que las diagonales no tienen una magnitud mayor a 1, 
        // limitamos la magnitud del vector.
        _direction = Vector3.ClampMagnitude(_direction, 1f);
        // Calculamos la velocidad deseada en base a la dirección y la velocidad
        _desiredVelocity = _direction * _movementSpeed;

        // Sitúa el checker de colisión en la dirección a la que deseamos mover el tanque
        // haciendo uso de un vector 3 temporal para respetar la altura configurada en el objeto vacío
        Vector3 temp = transform.position + _direction * _checkDistance;
        temp.y = _checkPoint.position.y;
        _checkPoint.position = temp;
        // Si hay colisión paramos el movmiento
        if (_isWalled)
        {
            _desiredVelocity = Vector3.zero;
        }
        // Solo aplicamos y rotamos al jugador en caso de cumplir las siguientes condiciones:
        // - Que haya input
        // - Que no haya detección de colisión frontal
        // - Que estemos tocando el suelo
        if((_horizontal != 0f || _vertical != 0f) && !_isWalled && _grounded)
        {
            // Aplicamos el movimiento
            _rb.linearVelocity = Vector3.MoveTowards(_rb.linearVelocity,
                                                    _desiredVelocity,
                                                    Time.deltaTime * _acceleration);
        } 
    }
    /// <summary>
    ///  Detecta si hay un collider del layer indicado en contacto con el checker de 
    /// colisión frontal ( de desplazamiento).
    /// </summary>
    private void CollisionPreDetection()
    {
        Collider[] colliderBuffered = new Collider[1];
        Physics.OverlapSphereNonAlloc(_checkPoint.position,
                                    _checkSize,
                                    colliderBuffered,
                                    _checkLayer);
        _isWalled = colliderBuffered[0] != null;
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
    private void Aiming() 
    {
        if (_usingGamepad)
        {
            AimingWithStick();
        }
        else
        {
            AimingWithMouse();
        }
    }

    private void AimingWithMouse()
    {
        if(Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, _camRayLenght, _pointerLayer))
        {
            _aimingPivot.position = new Vector3(hit.point.x, _aimingPivot.position.y, hit.point.z);
        }
    }

    private void AimingWithStick()
    {
        Vector3 direction = new Vector3(_lookInput.x, 0f, _lookInput.y);
        
        if(direction.sqrMagnitude < 0.01f) return;
        direction.Normalize();

        Vector3 target = transform.position + direction;

        _aimingPivot.position = new Vector3(target.x, _aimingPivot.position.y, target.z);
    }
    #endregion
}
