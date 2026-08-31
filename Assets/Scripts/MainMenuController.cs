using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the scene to load when clicking Start Game")]
    [SerializeField] private string sceneName = "Dev_Restaurant_Flow";

    [Tooltip("The name of the scene to load when clicking Credits")]
    [SerializeField] private string creditsSceneName = "EndCredit";

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
        string target = sceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "restaurant-scene";
        }

        Debug.Log($"[MainMenuController] Starting game. Loading scene: '{target}'");
        SceneManager.LoadScene(target);
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
    /// Loads the credits scene configured in the creditsSceneName serialized field.
    /// </summary>
    public void OpenCredits()
    {
        if (string.IsNullOrEmpty(creditsSceneName))
        {
            Debug.LogError("[MainMenuController] Credits scene name is empty!", this);
            return;
        }

        Debug.Log($"[MainMenuController] Opening credits. Loading scene: '{creditsSceneName}'");
        SceneManager.LoadScene(creditsSceneName);
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
