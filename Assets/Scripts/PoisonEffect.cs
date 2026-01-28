using UnityEngine;

// Эффект яда (DOT - Damage Over Time).
// Наносит периодический урон в течение заданного времени.
// Добавляется динамически через Projectile при попадании.
public class PoisonEffect : MonoBehaviour
{
    private float damagePerTick;
    private float tickInterval;
    private float duration;
    private float remainingTime;
    private float nextTickTime;

    private Health health;
    private bool isActive = false;

    // Активен ли эффект яда
    public bool IsActive => isActive;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    // Применить эффект яда.
    // Если яд уже активен — обновляет длительность (не суммирует).
    // "damagePerTick = Урон за тик
    // "tickInterval" = Интервал между тиками (сек)
    // "duration" = Общая длительность эффекта (сек)
    public void Apply(float damagePerTick, float tickInterval, float duration)
    {
        this.damagePerTick = damagePerTick;
        this.tickInterval = tickInterval;
        this.duration = duration;
        this.remainingTime = duration;
        this.nextTickTime = Time.time + tickInterval;

        if (!isActive)
        {
            isActive = true;
            Debug.Log($"{gameObject.name}: Отравлен! ({damagePerTick} урона каждые {tickInterval}с, длительность {duration}с)");
        }
        else
        {
            Debug.Log($"{gameObject.name}: Яд обновлён! Длительность сброшена на {duration}с");
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (health == null || !health.IsAlive)
        {
            RemovePoison();
            return;
        }

        // Отсчитываем время
        remainingTime -= Time.deltaTime;

        // Тик урона
        if (Time.time >= nextTickTime)
        {
            health.TakeDamage(damagePerTick);
            Debug.Log($"{gameObject.name}: Урон от яда: {damagePerTick} (осталось {remainingTime:F1}с)");
            nextTickTime = Time.time + tickInterval;
        }

        // Яд закончился
        if (remainingTime <= 0f)
        {
            RemovePoison();
        }
    }

    // Снять эффект яда
    public void RemovePoison()
    {
        isActive = false;
        Debug.Log($"{gameObject.name}: Яд прошёл.");
        Destroy(this); // Удаляем компонент
    }
}
