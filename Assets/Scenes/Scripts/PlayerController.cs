using UnityEngine;

// Контроллер игрока с движением и прыжком.
// Управление: WASD/стрелки для движения, Space для прыжка.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Скорость движения игрока")]
    public float moveSpeed = 10f;

    [Tooltip("Скорость поворота")]
    public float rotationSpeed = 100f;

    [Header("Настройки прыжка")]
    [Tooltip("Сила прыжка")]
    public float jumpForce = 8f;

    [Tooltip("Расстояние проверки земли (для капсулы ~1.1)")]
    public float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        CheckGround();
        Move();
        Jump();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // Проверка нахождения на земле
    private void CheckGround()
    {
        // Raycast от центра капсулы вниз
        // Для стандартной капсулы (высота 2) центр на высоте 1 от пола
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance
        );
    }

    // Визуализация луча в Scene View для отладки
    private void OnDrawGizmos()
    {
        // Зелёный если на земле, красный если в воздухе
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.down * groundCheckDistance
        );
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}