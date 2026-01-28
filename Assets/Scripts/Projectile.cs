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

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Запускаем снаряд вперёд
        rb.linearVelocity = transform.forward * speed;

        // Уничтожаем через время жизни
        Destroy(gameObject, lifetime);
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
