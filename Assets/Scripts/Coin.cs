using UnityEngine;

// Скрипт монетки - вращение и визуальные эффекты.
// Монетка автоматически вращается для привлечения внимания.
public class Coin : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Скорость вращения монетки")]
    public float rotationSpeed = 100f;

    [Tooltip("Ось вращения")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Настройки парения")]
    [Tooltip("Включить эффект парения вверх-вниз")]
    public bool enableHover = true;

    [Tooltip("Амплитуда парения")]
    public float hoverAmplitude = 0.2f;

    [Tooltip("Скорость парения")]
    public float hoverSpeed = 2f;

    [Header("Стоимость монетки")]
    [Tooltip("Сколько очков даёт монетка")]
    public int value = 1;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вращение монетки
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        // Эффект парения (синусоида)
        if (enableHover)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );
        }
    }
}
