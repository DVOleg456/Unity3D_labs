using UnityEngine;

// Наносит урон при контакте (шипы, ловушки, опасные стены).
// Можно настроить разовый или периодический урон.
public class DamageOnContact : MonoBehaviour
{
    [Header("Настройки урона")]
    [Tooltip("Урон за одно срабатывание")]
    public float damage = 10f;

    [Tooltip("Периодический урон (пока игрок касается)")]
    public bool continuousDamage = false;

    [Tooltip("Интервал между тиками урона (сек)")]
    public float damageInterval = 0.5f;

    [Header("Фильтр")]
    [Tooltip("Тег объектов, которым наносится урон")]
    public string targetTag = "Player";

    private float lastDamageTime;

    private void OnCollisionEnter(Collision collision)
    {
        TryDealDamage(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (continuousDamage)
        {
            TryDealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamage(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (continuousDamage)
        {
            TryDealDamage(other.gameObject);
        }
    }

    private void TryDealDamage(GameObject target)
    {
        if (!target.CompareTag(targetTag))
            return;

        // Проверяем интервал между тиками
        if (Time.time - lastDamageTime < damageInterval)
            return;

        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}
