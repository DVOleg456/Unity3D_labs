using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// UI экрана смерти игрока.
// Показывает панель Game Over при смерти и позволяет перезапустить уровень.
public class GameOverUI : MonoBehaviour
{
    [Header("UI элементы")]
    [Tooltip("Панель Game Over (Canvas/Panel)")]
    public GameObject gameOverPanel;

    [Tooltip("Текст заголовка (опционально)")]
    public Text gameOverText;

    [Tooltip("Кнопка перезапуска (опционально)")]
    public Button restartButton;

    [Header("Настройки")]
    [Tooltip("Ставить игру на паузу при смерти")]
    public bool pauseOnDeath = true;

    [Tooltip("Автоматически найти игрока по тегу")]
    public string playerTag = "Player";

    private Health playerHealth;

    private void Start()
    {
        // Скрываем панель в начале
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Находим игрока и подписываемся на событие смерти
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.OnDied += ShowGameOver;
            }
        }

        // Настраиваем кнопку перезапуска
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события
        if (playerHealth != null)
        {
            playerHealth.OnDied -= ShowGameOver;
        }
    }

    // Показать экран Game Over
    public void ShowGameOver()
    {
        Debug.Log("Game Over!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Ставим игру на паузу
        if (pauseOnDeath)
        {
            Time.timeScale = 0f;
        }

        // Показываем курсор для UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Перезапустить текущий уровень
    public void RestartLevel()
    {
        // Восстанавливаем время
        Time.timeScale = 1f;

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Выход в главное меню (или закрытие игры)
    public void QuitGame()
    {
        Time.timeScale = 1f;

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
