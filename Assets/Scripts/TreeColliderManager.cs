using UnityEngine;
using System.Collections.Generic;

// Управляет коллайдерами деревьев в зависимости от расстояния до игрока.
// Включает коллайдеры только у ближайших деревьев для оптимизации.
public class TreeColliderManager : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Игрок (если не указан — ищет по тегу 'Player')")]
    public Transform player;

    [Tooltip("Генератор леса (если не указан — ищет автоматически)")]
    public ForestGenerator forestGenerator;

    [Header("Настройки")]
    [Tooltip("Радиус активации коллайдеров")]
    public float activationRadius = 10f;

    [Tooltip("Интервал обновления (сек) — для оптимизации")]
    public float updateInterval = 0.2f;

    [Header("Отладка")]
    [Tooltip("Показывать радиус в Scene View")]
    public bool showDebugRadius = true;

    private List<TreeColliderData> treeColliders = new List<TreeColliderData>();
    private float nextUpdateTime;

    // Данные о коллайдере дерева
    private struct TreeColliderData
    {
        public Transform transform;
        public Collider collider;
    }

    private void Start()
    {
        // Находим игрока
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Находим генератор леса
        if (forestGenerator == null)
        {
            forestGenerator = FindFirstObjectByType<ForestGenerator>();
        }

        // Собираем коллайдеры деревьев через небольшую задержку
        // (чтобы ForestGenerator успел создать деревья)
        Invoke(nameof(CollectTreeColliders), 0.5f);
    }

    // Собрать все коллайдеры деревьев
    public void CollectTreeColliders()
    {
        treeColliders.Clear();

        if (forestGenerator != null)
        {
            List<GameObject> trees = forestGenerator.GetSpawnedTrees();

            foreach (GameObject tree in trees)
            {
                if (tree == null) continue;

                Collider col = tree.GetComponent<Collider>();
                if (col == null)
                {
                    col = tree.GetComponentInChildren<Collider>();
                }

                if (col != null)
                {
                    treeColliders.Add(new TreeColliderData
                    {
                        transform = tree.transform,
                        collider = col
                    });

                    // Изначально выключаем коллайдер
                    col.enabled = false;
                }
            }
        }
        else
        {
            // Если нет генератора — ищем все деревья по тегу
            GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

            foreach (GameObject tree in trees)
            {
                Collider col = tree.GetComponent<Collider>();
                if (col != null)
                {
                    treeColliders.Add(new TreeColliderData
                    {
                        transform = tree.transform,
                        collider = col
                    });

                    col.enabled = false;
                }
            }
        }

        Debug.Log($"TreeColliderManager: Найдено {treeColliders.Count} деревьев с коллайдерами");
    }

    private void Update()
    {
        if (player == null) return;
        if (treeColliders.Count == 0) return;

        // Обновляем с интервалом для оптимизации
        if (Time.time < nextUpdateTime) return;
        nextUpdateTime = Time.time + updateInterval;

        UpdateColliders();
    }

    // Обновить состояние коллайдеров
    private void UpdateColliders()
    {
        Vector3 playerPos = player.position;
        float sqrRadius = activationRadius * activationRadius;

        foreach (TreeColliderData data in treeColliders)
        {
            if (data.transform == null || data.collider == null) continue;

            float sqrDistance = (data.transform.position - playerPos).sqrMagnitude;
            bool shouldBeEnabled = sqrDistance <= sqrRadius;

            if (data.collider.enabled != shouldBeEnabled)
            {
                data.collider.enabled = shouldBeEnabled;
            }
        }
    }

    // Визуализация радиуса активации
    private void OnDrawGizmos()
    {
        if (!showDebugRadius) return;

        Transform target = player;
        if (target == null && Application.isPlaying)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(target.position, activationRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position, activationRadius);
        }
    }
}
