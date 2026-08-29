using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the scene to load when clicking Start Game")]
    [SerializeField] private string sceneName = "restaurant-scene";

    private void Awake()
    {
        // Ensure the cursor is unlocked and visible when in the menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Loads the scene configured in the sceneName serialized field.
    /// </summary>
    public void StartGame()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[MainMenuController] Scene name is empty! Please assign a scene name in the Inspector.", this);
            return;
        }

        Debug.Log($"[MainMenuController] Starting game. Loading scene: '{sceneName}'");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Overload that allows passing a specific scene name directly.
    /// </summary>
    public void StartGame(string targetSceneName)
    {
        string sceneToLoad = !string.IsNullOrEmpty(targetSceneName) ? targetSceneName : sceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("[MainMenuController] Scene name is empty!", this);
            return;
        }

        Debug.Log($"[MainMenuController] Starting game. Loading scene: '{sceneToLoad}'");
        SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Exits the game application or stops playmode in the Unity Editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[MainMenuController] Quitting game application.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
