using UnityEngine;
using System.Collections.Generic;

// Генератор леса с GPU Instancing для эффективного рендеринга.
// Создаёт множество деревьев на Terrain, рендерит через GPU.
public class ForestGenerator : MonoBehaviour
{
    [Header("Terrain")]
    [Tooltip("Terrain для размещения деревьев (если не указан — ищет автоматически)")]
    public Terrain terrain;

    [Header("Префабы деревьев")]
    [Tooltip("Список префабов деревьев для спавна")]
    public GameObject[] treePrefabs;

    [Header("Настройки генерации")]
    [Tooltip("Количество деревьев")]
    public int treeCount = 500;

    [Tooltip("Минимальное расстояние между деревьями")]
    public float minSpacing = 2f;

    [Tooltip("Область спавна (если нет Terrain)")]
    public Vector2 spawnArea = new Vector2(100f, 100f);

    [Header("Вариативность")]
    [Tooltip("Минимальный масштаб дерева")]
    public float minScale = 0.8f;

    [Tooltip("Максимальный масштаб дерева")]
    public float maxScale = 1.5f;

    [Tooltip("Случайный поворот по Y")]
    public bool randomRotation = true;

    [Header("GPU Instancing")]
    [Tooltip("Использовать GPU Instancing (рекомендуется)")]
    public bool useGPUInstancing = true;

    [Tooltip("Mesh дерева для GPU Instancing")]
    public Mesh treeMesh;

    [Tooltip("Материал с поддержкой GPU Instancing")]
    public Material treeMaterial;

    // Список позиций деревьев для GPU Instancing
    private List<Matrix4x4> treeMatrices = new List<Matrix4x4>();
    private List<GameObject> spawnedTrees = new List<GameObject>();

    // Для батчинга (GPU Instancing поддерживает до 1023 объектов за вызов)
    private const int BATCH_SIZE = 1023;

    private void Start()
    {
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        GenerateForest();
    }

    private void Update()
    {
        // Если используем GPU Instancing — рендерим каждый кадр
        if (useGPUInstancing && treeMesh != null && treeMaterial != null)
        {
            RenderTreesGPU();
        }
    }

    // Генерация леса
    public void GenerateForest()
    {
        ClearForest();

        List<Vector3> positions = GeneratePositions();

        if (useGPUInstancing && treeMesh != null && treeMaterial != null)
        {
            // GPU Instancing — сохраняем матрицы трансформации
            GenerateGPUInstances(positions);
            Debug.Log($"ForestGenerator: Создано {treeMatrices.Count} деревьев (GPU Instancing)");
        }
        else
        {
            // Обычные GameObject'ы
            GenerateGameObjects(positions);
            Debug.Log($"ForestGenerator: Создано {spawnedTrees.Count} деревьев (GameObjects)");
        }
    }

    // Генерация позиций деревьев
    private List<Vector3> GeneratePositions()
    {
        List<Vector3> positions = new List<Vector3>();
        int maxAttempts = treeCount * 10;
        int attempts = 0;

        while (positions.Count < treeCount && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPos = GetRandomPosition();

            // Проверяем минимальное расстояние до других деревьев
            bool tooClose = false;
            foreach (Vector3 existingPos in positions)
            {
                if (Vector3.Distance(randomPos, existingPos) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                positions.Add(randomPos);
            }
        }

        return positions;
    }

    // Получить случайную позицию на Terrain или в области
    private Vector3 GetRandomPosition()
    {
        Vector3 position;

        if (terrain != null)
        {
            // Позиция на Terrain
            Vector3 terrainPos = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            float x = Random.Range(terrainPos.x, terrainPos.x + terrainSize.x);
            float z = Random.Range(terrainPos.z, terrainPos.z + terrainSize.z);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;

            position = new Vector3(x, y, z);
        }
        else
        {
            // Позиция в заданной области
            float x = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
            float z = Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
            position = new Vector3(x, 0f, z);
        }

        return position;
    }

    // Генерация матриц для GPU Instancing
    private void GenerateGPUInstances(List<Vector3> positions)
    {
        treeMatrices.Clear();

        foreach (Vector3 pos in positions)
        {
            float scale = Random.Range(minScale, maxScale);
            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, Vector3.one * scale);
            treeMatrices.Add(matrix);
        }
    }

    // Рендеринг деревьев через GPU Instancing
    private void RenderTreesGPU()
    {
        // Разбиваем на батчи по 1023 объекта
        for (int i = 0; i < treeMatrices.Count; i += BATCH_SIZE)
        {
            int count = Mathf.Min(BATCH_SIZE, treeMatrices.Count - i);
            Matrix4x4[] batch = new Matrix4x4[count];

            for (int j = 0; j < count; j++)
            {
                batch[j] = treeMatrices[i + j];
            }

            Graphics.DrawMeshInstanced(treeMesh, 0, treeMaterial, batch);
        }
    }

    // Генерация обычных GameObject'ов (без GPU Instancing)
    private void GenerateGameObjects(List<Vector3> positions)
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogError("ForestGenerator: Префабы деревьев не назначены!");
            return;
        }

        foreach (Vector3 pos in positions)
        {
            // Выбираем случайный префаб
            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

            float scale = Random.Range(minScale, maxScale);
            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            GameObject tree = Instantiate(prefab, pos, rotation);
            tree.transform.localScale = Vector3.one * scale;
            tree.transform.parent = transform;

            spawnedTrees.Add(tree);
        }
    }

    // Очистка леса
    public void ClearForest()
    {
        foreach (GameObject tree in spawnedTrees)
        {
            if (tree != null)
                Destroy(tree);
        }
        spawnedTrees.Clear();
        treeMatrices.Clear();
    }

    // Получить список созданных деревьев (для TreeColliderManager)
    public List<GameObject> GetSpawnedTrees()
    {
        return spawnedTrees;
    }

    // Визуализация области спавна в редакторе
    private void OnDrawGizmosSelected()
    {
        if (terrain == null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, 10f, spawnArea.y));
        }
    }
}
