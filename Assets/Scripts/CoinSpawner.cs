using UnityEngine;
using System.Collections.Generic;

// Система спавна монет на тайлы лабиринта.
// Можно указать точки спавна вручную или использовать автоматический поиск тайлов.
public class CoinSpawner : MonoBehaviour
{
    [Header("Префаб монетки")]
    [Tooltip("Префаб монетки для спавна")]
    public GameObject coinPrefab;

    [Header("Режим спавна")]
    [Tooltip("Использовать ручные точки спавна")]
    public bool useManualSpawnPoints = false;

    [Tooltip("Ручные точки спавна (если useManualSpawnPoints = true)")]
    public Transform[] manualSpawnPoints;

    [Header("Автоматический спавн")]
    [Tooltip("Тег тайлов пола лабиринта")]
    public string floorTileTag = "FloorTile";

    [Tooltip("Высота спавна монетки над тайлом")]
    public float spawnHeight = 1f;

    [Tooltip("Шанс спавна монетки на тайле (0-1)")]
    [Range(0f, 1f)]
    public float spawnChance = 0.3f;

    [Header("Ограничения")]
    [Tooltip("Максимальное количество монет")]
    public int maxCoins = 20;

    [Tooltip("Минимальное расстояние между монетами")]
    public float minDistanceBetweenCoins = 2f;

    private List<GameObject> spawnedCoins = new List<GameObject>();

    private void Start()
    {
        SpawnCoins();
    }

    // Запуск спавна монет
    public void SpawnCoins()
    {
        if (coinPrefab == null)
        {
            Debug.LogError("CoinSpawner: Префаб монетки не назначен!");
            return;
        }

        ClearCoins();

        if (useManualSpawnPoints)
        {
            SpawnAtManualPoints();
        }
        else
        {
            SpawnOnFloorTiles();
        }

        Debug.Log($"CoinSpawner: Создано {spawnedCoins.Count} монет");
    }

    // Спавн на ручных точках
    private void SpawnAtManualPoints()
    {
        foreach (Transform point in manualSpawnPoints)
        {
            if (point != null && spawnedCoins.Count < maxCoins)
            {
                SpawnCoinAt(point.position);
            }
        }
    }

    // Автоматический спавн на тайлах пола
    private void SpawnOnFloorTiles()
    {
        // Находим все тайлы пола
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag(floorTileTag);

        if (floorTiles.Length == 0)
        {
            Debug.LogWarning($"CoinSpawner: Не найдены объекты с тегом '{floorTileTag}'");
            return;
        }

        // Перемешиваем массив для случайного порядка
        ShuffleArray(floorTiles);

        foreach (GameObject tile in floorTiles)
        {
            if (spawnedCoins.Count >= maxCoins)
                break;

            // Проверяем шанс спавна
            if (Random.value > spawnChance)
                continue;

            // Вычисляем позицию спавна
            Vector3 spawnPos = tile.transform.position + Vector3.up * spawnHeight;

            // Проверяем минимальное расстояние до других монет
            if (IsTooCloseToOtherCoins(spawnPos))
                continue;

            SpawnCoinAt(spawnPos);
        }
    }

    // Создание монетки в указанной позиции
    private void SpawnCoinAt(Vector3 position)
    {
        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
        coin.transform.parent = transform; // Организуем в иерархии
        spawnedCoins.Add(coin);
    }

    // Проверка расстояния до других монет
    private bool IsTooCloseToOtherCoins(Vector3 position)
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
            {
                float distance = Vector3.Distance(position, coin.transform.position);
                if (distance < minDistanceBetweenCoins)
                    return true;
            }
        }
        return false;
    }

    // Перемешивание массива (Fisher-Yates)
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    // Удаление всех монет
    public void ClearCoins()
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
                Destroy(coin);
        }
        spawnedCoins.Clear();
    }

    // Респавн монет (для рестарта уровня)
    public void RespawnCoins()
    {
        ClearCoins();
        SpawnCoins();
    }
}
