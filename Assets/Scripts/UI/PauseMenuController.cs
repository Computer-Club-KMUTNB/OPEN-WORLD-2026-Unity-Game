using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenuController : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private GameObject mainPausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Navigation")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("State")]
    public bool isPaused = false;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisible;

    private void Awake()
    {
        // 1. De-duplicate AudioListener if loaded additively
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (listeners.Length > 1)
        {
            AudioListener myListener = GetComponentInChildren<AudioListener>(true);
            if (myListener == null)
            {
                Camera cam = GetComponentInChildren<Camera>(true);
                if (cam != null) myListener = cam.GetComponent<AudioListener>();
            }
            if (myListener != null)
            {
                myListener.enabled = false;
            }
        }

        // 2. De-duplicate EventSystem if loaded additively
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        if (eventSystems.Length > 1)
        {
            foreach (var es in eventSystems)
            {
                if (es != null && es.gameObject.scene == gameObject.scene)
                {
                    Destroy(es.gameObject);
                    break;
                }
            }
        }
    }

    private void Start()
    {
        // Ensure menu starts closed unless testing
        if (pauseMenuRoot != null && !isPaused)
        {
            pauseMenuRoot.SetActive(false);
        }
    }

    private void Update()
    {
        bool escapePressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escapePressed = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            escapePressed = true;
        }
#endif

        if (escapePressed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        previousCursorLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
        if (mainPausePanel != null) mainPausePanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);

        // If loaded additively, unload the pause menu scene so background and canvas are completely removed
        Scene currentScene = gameObject.scene;
        if (currentScene.isLoaded && SceneManager.sceneCount > 1)
        {
            SceneManager.UnloadSceneAsync(currentScene);
        }
    }

    public void OpenSettings()
    {
        if (mainPausePanel != null) mainPausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPausePanel != null) mainPausePanel.SetActive(true);
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

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
