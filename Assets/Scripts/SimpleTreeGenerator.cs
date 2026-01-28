using UnityEngine;

// Создаёт простое дерево из примитивов (для тестирования).
// Запусти в редакторе через контекстное меню.
public class SimpleTreeGenerator : MonoBehaviour
{
    [Header("Настройки ствола")]
    public float trunkHeight = 2f;
    public float trunkRadius = 0.2f;
    public Color trunkColor = new Color(0.4f, 0.26f, 0.13f); // Коричневый

    [Header("Настройки кроны")]
    public float crownRadius = 1.5f;
    public Color crownColor = new Color(0.13f, 0.55f, 0.13f); // Зелёный

    [Header("Вариации")]
    public bool randomize = true;

    // Создать дерево как дочерний объект
    [ContextMenu("Создать дерево")]
    public GameObject CreateTree()
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.parent = transform;
        tree.transform.localPosition = Vector3.zero;

        float heightMult = randomize ? Random.Range(0.8f, 1.3f) : 1f;
        float widthMult = randomize ? Random.Range(0.8f, 1.2f) : 1f;

        // Ствол (цилиндр)
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.parent = tree.transform;
        trunk.transform.localPosition = new Vector3(0f, trunkHeight * heightMult / 2f, 0f);
        trunk.transform.localScale = new Vector3(
            trunkRadius * 2f * widthMult,
            trunkHeight * heightMult / 2f,
            trunkRadius * 2f * widthMult
        );

        // Материал ствола
        Renderer trunkRenderer = trunk.GetComponent<Renderer>();
        trunkRenderer.material = new Material(Shader.Find("Standard"));
        trunkRenderer.material.color = trunkColor;
        trunkRenderer.material.enableInstancing = true;

        // Крона (сфера)
        GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crown.name = "Crown";
        crown.transform.parent = tree.transform;
        crown.transform.localPosition = new Vector3(0f, trunkHeight * heightMult + crownRadius * widthMult * 0.5f, 0f);
        crown.transform.localScale = Vector3.one * crownRadius * 2f * widthMult;

        // Материал кроны
        Renderer crownRenderer = crown.GetComponent<Renderer>();
        crownRenderer.material = new Material(Shader.Find("Standard"));
        crownRenderer.material.color = crownColor;
        crownRenderer.material.enableInstancing = true;

        // Добавляем общий коллайдер на дерево (капсула)
        CapsuleCollider treeCollider = tree.AddComponent<CapsuleCollider>();
        treeCollider.center = new Vector3(0f, (trunkHeight * heightMult + crownRadius * widthMult) / 2f, 0f);
        treeCollider.height = trunkHeight * heightMult + crownRadius * 2f * widthMult;
        treeCollider.radius = Mathf.Max(trunkRadius, crownRadius * 0.5f) * widthMult;

        // Удаляем отдельные коллайдеры
        Destroy(trunk.GetComponent<Collider>());
        Destroy(crown.GetComponent<Collider>());

        // Тег для TreeColliderManager
        tree.tag = "Tree";

        return tree;
    }

    // Создать префаб дерева и сохранить ссылку
    [ContextMenu("Создать и назначить как префаб")]
    public void CreateTreePrefab()
    {
        GameObject tree = CreateTree();
        tree.transform.parent = null;
        Debug.Log("Дерево создано! Перетащи его в папку Assets/Prefabs чтобы сделать префаб.");
    }
}
