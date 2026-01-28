using UnityEngine;

// Универсальная система здоровья.
// Вешается на игрока или любой объект, которому можно нанести урон.
public class Health : MonoBehaviour
{
    [Header("Здоровье")]
    [Tooltip("Максимальное здоровье")]
    public float maxHealth = 100f;

    [Tooltip("Текущее здоровье")]
    [SerializeField] private float currentHealth;

    // Текущее здоровье (только чтение)
    public float CurrentHealth => currentHealth;

    // Жив ли объект
    public bool IsAlive => currentHealth > 0f;

    // Событие при получении урона (текущее HP)
    public event System.Action<float> OnDamaged;

    // Событие при смерти
    public event System.Action OnDied;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Нанести урон
    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log($"{gameObject.name} получил урон: {damage}. HP: {currentHealth}/{maxHealth}");

        OnDamaged?.Invoke(currentHealth);

        if (!IsAlive)
        {
            Die();
        }
    }

    // Восстановить здоровье
    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} исцелён на {amount}. HP: {currentHealth}/{maxHealth}");
    }

    // Смерть объекта
    private void Die()
    {
        Debug.Log($"{gameObject.name} погиб!");
        OnDied?.Invoke();
    }
}
