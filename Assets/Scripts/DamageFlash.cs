using UnityEngine;
using System.Collections;

// Визуальный эффект моргания красным при получении урона.
// Вешается на объект с компонентом Health.
[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    [Header("Настройки вспышки")]
    [Tooltip("Цвет вспышки при уроне")]
    public Color flashColor = Color.red;

    [Tooltip("Длительность вспышки (сек)")]
    public float flashDuration = 0.15f;

    [Tooltip("Количество морганий")]
    public int flashCount = 2;

    private Health health;
    private Renderer objectRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        health = GetComponent<Health>();
        objectRenderer = GetComponentInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (objectRenderer != null)
        {
            // Сохраняем оригинальный цвет
            originalColor = objectRenderer.material.color;
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие получения урона
        if (health != null)
        {
            health.OnDamaged += OnDamaged;
        }
    }

    private void OnDisable()
    {
        // Отписываемся от события
        if (health != null)
        {
            health.OnDamaged -= OnDamaged;
        }

        // Восстанавливаем цвет при отключении
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    // Вызывается при получении урона
    private void OnDamaged(float currentHealth)
    {
        if (objectRenderer == null) return;

        // Останавливаем предыдущую анимацию, если она есть
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    // Корутина моргания
    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            // Красный цвет
            objectRenderer.material.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // Оригинальный цвет
            objectRenderer.material.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        // Гарантируем возврат к оригинальному цвету
        objectRenderer.material.color = originalColor;
        flashCoroutine = null;
    }
}
