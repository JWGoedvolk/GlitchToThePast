using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ToBeDeletedPlayerMovementScript : MonoBehaviour
{
    #region Variables
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody _rb;
    private Vector3 _movementDirection;
    #endregion

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");  

        _movementDirection = new Vector3(moveX, 0f, moveZ).normalized;
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementDirection * moveSpeed * Time.fixedDeltaTime);
    }
}