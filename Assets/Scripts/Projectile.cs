using UnityEngine;

// Снаряд турели. Летит вперёд, наносит урон при попадании.
// Может накладывать эффект яда (DOT-урон).
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Настройки полёта")]
    [Tooltip("Скорость снаряда")]
    public float speed = 20f;

    [Tooltip("Время жизни снаряда (сек)")]
    public float lifetime = 5f;

    [Header("Настройки урона")]
    [Tooltip("Прямой урон при попадании")]
    public float damage = 15f;

    [Header("Яд (DOT-урон)")]
    [Tooltip("Накладывать эффект яда при попадании")]
    public bool applyPoison = true;

    [Tooltip("Урон яда за тик")]
    public float poisonDamagePerTick = 5f;

    [Tooltip("Интервал тиков яда (сек)")]
    public float poisonTickInterval = 1f;

    [Tooltip("Длительность яда (сек)")]
    public float poisonDuration = 5f;

    [Header("Фильтр")]
    [Tooltip("Тег цели")]
    public string targetTag = "Player";

    [Header("Визуализация")]
    [Tooltip("Включить след снаряда")]
    public bool enableTrail = true;

    [Tooltip("Цвет следа")]
    public Color trailColor = Color.yellow;

    [Tooltip("Ширина следа")]
    public float trailWidth = 0.3f;

    [Tooltip("Время жизни следа")]
    public float trailTime = 0.3f;

    private Rigidbody rb;
    private Transform target;
    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Если есть цель — летим к ней, иначе вперёд
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
            // Поворачиваем снаряд в направлении цели
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            direction = transform.forward;
        }

        rb.linearVelocity = direction * speed;

        // Создаём визуальный след
        if (enableTrail)
        {
            SetupTrail();
        }

        // Уничтожаем через время жизни
        Destroy(gameObject, lifetime);
    }

    // Настройка визуального следа снаряда
    private void SetupTrail()
    {
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }

        trail.time = trailTime;
        trail.startWidth = trailWidth;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = trailColor;
        trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
    }

    // Установить цель для наведения
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        // Наносим прямой урон
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"Снаряд попал! Урон: {damage}");

            // Накладываем яд
            if (applyPoison)
            {
                ApplyPoisonEffect(other.gameObject);
            }
        }

        // Уничтожаем снаряд при попадании
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);

                if (applyPoison)
                {
                    ApplyPoisonEffect(collision.gameObject);
                }
            }
        }

        // Уничтожаем снаряд при столкновении с чем угодно
        Destroy(gameObject);
    }

    // Наложить эффект яда на цель
    private void ApplyPoisonEffect(GameObject target)
    {
        PoisonEffect poison = target.GetComponent<PoisonEffect>();

        // Если яда ещё нет — добавляем компонент
        if (poison == null)
        {
            poison = target.AddComponent<PoisonEffect>();
        }

        // Применяем (или обновляем) яд
        poison.Apply(poisonDamagePerTick, poisonTickInterval, poisonDuration);

        Debug.Log($"Яд наложен! {poisonDamagePerTick} урона каждые {poisonTickInterval}с на {poisonDuration}с");
    }
}