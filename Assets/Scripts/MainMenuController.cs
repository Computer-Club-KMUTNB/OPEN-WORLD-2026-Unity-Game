using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The name of the scene to load for restaurant")]
    [SerializeField] private string restaurantSceneName = "Dev_Restaurant_Flow";

    [Tooltip("The name of the scene to load when clicking Credits")]
    [SerializeField] private string creditsSceneName = "EndCredit";

    [Header("Buttons")]
    [SerializeField] private GameObject continueButton;

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

        UpdateContinueButtonState();
    }

    private void OnEnable()
    {
        UpdateContinueButtonState();
    }

    public void UpdateContinueButtonState()
    {
        bool hasSave = SaveSystem.HasSaveFile();
        if (continueButton != null)
        {
            continueButton.SetActive(hasSave);
        }
    }

    /// <summary>
    /// Starts a fresh New Game: Money=0, All Meats=0, Vegetable=5, Rice=5, Day=1.
    /// Saves to savegame.json and loads the restaurant scene.
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log("[MainMenuController] Starting NEW GAME (Money:0, Meats:0, Veggie:5, Rice:5)...");
        SaveSystem.StartNewGame();

        LoadRestaurantScene();
    }

    /// <summary>
    /// Continues existing game by loading savegame.json and going to restaurant scene.
    /// </summary>
    public void ContinueGame()
    {
        Debug.Log("[MainMenuController] CONTINUING game from save file...");
        SaveSystem.Load();

        LoadRestaurantScene();
    }

    // Backwards compatible alias
    public void StartGame()
    {
        StartNewGame();
    }

    public void StartGame(string targetSceneName)
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            LoadRestaurantScene();
        }
    }

    private void LoadRestaurantScene()
    {
        string target = restaurantSceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "restaurant-scene";
        }

        Debug.Log($"[MainMenuController] Loading Restaurant Scene: '{target}'");
        SceneManager.LoadScene(target);
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
