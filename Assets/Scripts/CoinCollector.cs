using UnityEngine;

// Компонент игрока для сбора монет.
// Отслеживает количество собранных монет.
public class CoinCollector : MonoBehaviour
{
    [Header("Статистика")]
    [Tooltip("Текущее количество собранных монет")]
    [SerializeField] private int collectedCoins = 0;

    [Header("Настройки")]
    [Tooltip("Тег объектов-монет")]
    public string coinTag = "Coin";

    // Публичное свойство для получения количества монет
    public int CollectedCoins => collectedCoins;

    // Событие при сборе монетки (для UI и других систем)
    public event System.Action<int> OnCoinCollected;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что столкнулись с монеткой
        if (other.CompareTag(coinTag))
        {
            CollectCoin(other.gameObject);
        }
    }

    // Сбор монетки
    private void CollectCoin(GameObject coinObject)
    {
        // Получаем компонент Coin для определения стоимости
        Coin coin = coinObject.GetComponent<Coin>();
        int value = coin != null ? coin.value : 1;

        // Добавляем очки
        collectedCoins += value;

        // Выводим в консоль для отладки
        Debug.Log($"Монетка собрана! Всего: {collectedCoins}");

        // Вызываем событие (если кто-то подписан)
        OnCoinCollected?.Invoke(collectedCoins);

        // Уничтожаем монетку
        Destroy(coinObject);
    }

    // Сброс счётчика (например, при рестарте уровня)
    public void ResetCoins()
    {
        collectedCoins = 0;
        OnCoinCollected?.Invoke(collectedCoins);
    }

    // Добавить монеты вручную (для бонусов и т.д.)
    public void AddCoins(int amount)
    {
        collectedCoins += amount;
        OnCoinCollected?.Invoke(collectedCoins);
    }
}
