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

    [Header("Проверка видимости")]
    [Tooltip("Проверять линию видимости (не стрелять сквозь стены)")]
    public bool requireLineOfSight = true;

    [Tooltip("Слой препятствий (стены, объекты)")]
    public LayerMask obstacleLayer = ~0; // По умолчанию все слои

    private Transform target;
    private float nextFireTime;
    private bool hasLineOfSight = false;

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
            // Проверяем линию видимости
            hasLineOfSight = CheckLineOfSight();

            RotateTowardsTarget();

            // Стреляем только если видим цель
            if (!requireLineOfSight || hasLineOfSight)
            {
                TryShoot();
            }
        }
        else
        {
            hasLineOfSight = false;
        }
    }

    // Проверка линии видимости до цели (Raycast)
    private bool CheckLineOfSight()
    {
        Vector3 startPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 direction = target.position - startPos;
        float distance = direction.magnitude;

        // Пускаем луч от точки стрельбы к игроку
        if (Physics.Raycast(startPos, direction.normalized, out RaycastHit hit, distance, obstacleLayer))
        {
            // Если луч попал в игрока — видим его
            if (hit.transform == target)
            {
                return true;
            }
            // Если попали во что-то другое — есть препятствие
            return false;
        }

        // Луч ни во что не попал — путь свободен
        return true;
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

        // Передаём цель снаряду для точного наведения
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetTarget(target);
        }

        Debug.Log("Турель стреляет!");
    }

    // Визуализация радиуса обнаружения и линии видимости в Scene View
    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Линия видимости до игрока
        if (target != null)
        {
            Vector3 startPos = firePoint != null ? firePoint.position : transform.position;
            Gizmos.color = hasLineOfSight ? Color.green : Color.red;
            Gizmos.DrawLine(startPos, target.position);
        }
    }
}