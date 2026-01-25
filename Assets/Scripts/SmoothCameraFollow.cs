using UnityEngine;

// Скрипт плавного следования камеры за игроком с использованием Lerp.
// Камера всегда смотрит на игрока, но следует с небольшой задержкой,
// создавая эффект "отставания" от быстро движущегося игрока.
public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Цель слежения")]
    [Tooltip("Объект игрока, за которым следует камера")]
    public Transform target;

    [Header("Настройки позиции")]
    [Tooltip("Смещение камеры относительно игрока")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Настройки плавности")]
    [Tooltip("Скорость следования камеры (чем меньше - тем плавнее)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("Настройки поворота")]
    [Tooltip("Плавный поворот камеры к цели")]
    public bool smoothRotation = true;

    [Tooltip("Скорость поворота камеры")]
    [Range(1f, 20f)]
    public float rotationSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("SmoothCameraFollow: Цель не назначена!");
            return;
        }

        FollowTarget();
        LookAtTarget();
    }

    // Плавное перемещение камеры к желаемой позиции с использованием Lerp
    private void FollowTarget()
    {
        // Вычисляем желаемую позицию камеры
        Vector3 desiredPosition = target.position + offset;

        // Используем Lerp для плавного перемещения
        // Камера "догоняет" игрока, но никогда не достигает его мгновенно
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position, // Текущая позиция камеры
            desiredPosition, // Желаемая позиция
            smoothSpeed // Коэффициент интерполяции
        );

        // Применяем новую позицию
        transform.position = smoothedPosition;
    }

    // Поворот камеры в сторону игрока
    private void LookAtTarget()
    {
        if (smoothRotation)
        {
            // Плавный поворот с использованием Slerp
            Vector3 direction = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Мгновенный поворот к цели
            transform.LookAt(target);
        }
    }

    // Установка цели слежения из кода
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Мгновенная телепортация камеры к цели (без Lerp)
    // Полезно при респавне игрока или смене сцены
    public void SnapToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
