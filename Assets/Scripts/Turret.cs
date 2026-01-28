using UnityEngine;

// Турель, которая обнаруживает игрока и стреляет снарядами.
// Поворачивается к цели и стреляет с заданным интервалом.
public class Turret : MonoBehaviour
{
    [Header("Обнаружение")]
    [Tooltip("Дальность обнаружения игрока")]
    public float detectionRange = 15f;

    [Tooltip("Тег цели")]
    public string targetTag = "Player";

    [Header("Стрельба")]
    [Tooltip("Префаб снаряда")]
    public GameObject projectilePrefab;

    [Tooltip("Точка, из которой вылетает снаряд")]
    public Transform firePoint;

    [Tooltip("Интервал между выстрелами (сек)")]
    public float fireRate = 2f;

    [Header("Поворот")]
    [Tooltip("Скорость поворота к цели")]
    public float rotationSpeed = 5f;

    [Tooltip("Поворачивать только по горизонтали")]
    public bool horizontalOnly = true;

    private Transform target;
    private float nextFireTime;

    private void Start()
    {
        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        if (player != null)
        {
            target = player.transform;
        }

        // Если firePoint не назначен, используем позицию турели
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Проверяем дальность обнаружения
        if (distanceToTarget <= detectionRange)
        {
            RotateTowardsTarget();
            TryShoot();
        }
    }

    // Поворот турели к цели
    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;

        // Если только горизонтальный поворот — убираем вертикальную составляющую
        if (horizontalOnly)
        {
            direction.y = 0f;
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // Попытка выстрелить
    private void TryShoot()
    {
        if (projectilePrefab == null) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Выстрел снарядом
    private void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Debug.Log("Турель стреляет!");
    }

    // Визуализация радиуса обнаружения в Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
