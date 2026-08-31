using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreditsController : MonoBehaviour
{
    [Header("JSON Data Source")]
    [Tooltip("Drag & drop your credits JSON file here (TextAsset)")]
    [SerializeField] private TextAsset creditsJsonFile;
    [Tooltip("Fallback file path if TextAsset is not assigned (relative to project or StreamingAssets)")]
    [SerializeField] private string jsonFilePath = "Assets/Data/Credits/credits.json";

    [Header("UI References - Scrolling Credits")]
    [SerializeField] private RectTransform scrollContent;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private CanvasGroup contentCanvasGroup;

    [Header("UI References - Final Game Title Finale")]
    [SerializeField] private CanvasGroup finalTitleCanvasGroup;
    [SerializeField] private TextMeshProUGUI finalTitleText;
    [SerializeField] private TextMeshProUGUI finalSubtitleText;
    [SerializeField] private TextMeshProUGUI finalFooterText;

    [Header("UI References - Controls")]
    [SerializeField] private Button skipButton;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Scrolling Configuration")]
    [Tooltip("Base upward scrolling speed in units/sec")]
    [SerializeField] private float scrollSpeed = 65f;
    [Tooltip("Multiplier applied when user holds Space, Left Mouse, or Down Arrow")]
    [SerializeField] private float fastForwardMultiplier = 3.5f;
    [Tooltip("Vertical starting position for the credits (0 = start at center of screen)")]
    [SerializeField] private float startPositionYOffset = 0f;
    [Tooltip("Pause in seconds before the upward scroll begins (allows reading initial title)")]
    [SerializeField] private float startDelay = 1.5f;
    [Tooltip("Duration of the initial fade-in")]
    [SerializeField] private float fadeInDuration = 1.0f;

    [Header("Final Title Screen Configuration")]
    [Tooltip("Duration for the big final title to fade in")]
    [SerializeField] private float finalTitleFadeInDuration = 1.2f;
    [Tooltip("How long the big title stays on screen")]
    [SerializeField] private float finalTitleDisplayDuration = 3.2f;
    [Tooltip("Duration for the big final title to fade out before returning to menu")]
    [SerializeField] private float finalTitleFadeOutDuration = 1.0f;

    [Header("Navigation")]
    [Tooltip("Name of the scene to load once credits end or when skipped")]
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private bool autoLoadNextScene = true;

    [Header("Styling Colors (Hex Codes)")]
    [SerializeField] private string titleColorHex = "#F6AD55";
    [SerializeField] private string subtitleColorHex = "#A0AEC0";
    [SerializeField] private string sectionHeaderColorHex = "#63B3ED";
    [SerializeField] private string roleTitleColorHex = "#CBD5E0";
    [SerializeField] private string namesColorHex = "#FFFFFF";
    [SerializeField] private string footerColorHex = "#ECC94B";

    private bool isScrolling = false;
    private bool isEnding = false;
    private float startPositionY;
    private float targetEndPositionY;
    private CreditsData loadedData;

    private void Awake()
    {
        // Free and show mouse cursor in credits
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (finalTitleCanvasGroup != null)
        {
            finalTitleCanvasGroup.alpha = 0f;
            finalTitleCanvasGroup.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCredits);
        }

        StartCoroutine(RunCreditsSequence());
    }

    private void Update()
    {
        // Allow pressing Escape or Q to skip
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
        {
            SkipCredits();
        }
    }

    private IEnumerator RunCreditsSequence()
    {
        // 1. Load and parse JSON
        loadedData = LoadCreditsData();
        if (loadedData == null)
        {
            Debug.LogError("[CreditsController] Failed to load credits data. Using fallback text.");
            loadedData = GetFallbackCreditsData();
        }

        // 2. Set Final Big Title components
        if (finalTitleText != null)
        {
            finalTitleText.text = loadedData.gameTitle;
        }
        if (finalSubtitleText != null)
        {
            finalSubtitleText.text = loadedData.gameSubtitle;
        }
        if (finalFooterText != null)
        {
            finalFooterText.text = !string.IsNullOrEmpty(loadedData.footer) ? loadedData.footer : "THANK YOU FOR PLAYING!";
        }

        // 3. Format and assign rich text to scrolling TextMeshPro
        string formattedText = BuildFormattedCreditsString(loadedData);
        if (creditsText != null)
        {
            creditsText.text = formattedText;
            creditsText.ForceMeshUpdate();
        }

        // 4. Setup canvas group for initial fade-in
        if (contentCanvasGroup != null)
        {
            contentCanvasGroup.alpha = 0f;
        }

        // Wait a frame for UI layout update
        yield return null;

        // 5. Calculate start and end scroll positions
        float screenHeight = 1080f;
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null && parentCanvas.rootCanvas != null)
        {
            RectTransform canvasRect = parentCanvas.rootCanvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                screenHeight = canvasRect.rect.height;
            }
        }

        float textHeight = creditsText != null ? creditsText.preferredHeight : 2000f;
        if (scrollContent != null)
        {
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, textHeight + 100f);

            // Start at center
            startPositionY = startPositionYOffset;
            targetEndPositionY = textHeight + screenHeight * 0.5f + 100f;

            scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, startPositionY);
        }

        // Fade in at center
        if (contentCanvasGroup != null && fadeInDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                contentCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }
            contentCanvasGroup.alpha = 1f;
        }
        else if (contentCanvasGroup != null)
        {
            contentCanvasGroup.alpha = 1f;
        }

        // Pause briefly at center so player can read initial credits before scrolling
        yield return new WaitForSeconds(startDelay);

        // 6. Auto Scroll Loop
        isScrolling = true;
        while (isScrolling && scrollContent != null && scrollContent.anchoredPosition.y < targetEndPositionY)
        {
            bool isFastForwarding = Input.GetKey(KeyCode.Space) || 
                                   Input.GetMouseButton(0) || 
                                   Input.GetKey(KeyCode.DownArrow);

            float currentSpeed = scrollSpeed * (isFastForwarding ? fastForwardMultiplier : 1.0f);
            float newY = scrollContent.anchoredPosition.y + (currentSpeed * Time.deltaTime);

            scrollContent.anchoredPosition = new Vector2(scrollContent.anchoredPosition.x, newY);
            yield return null;
        }

        isScrolling = false;

        // 7. Fade out scrolling credits
        if (contentCanvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 0.8f;
            float initialAlpha = contentCanvasGroup.alpha;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                contentCanvasGroup.alpha = Mathf.Lerp(initialAlpha, 0f, elapsed / duration);
                yield return null;
            }
            contentCanvasGroup.alpha = 0f;
        }

        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }

        // 8. Big Game Title Grand Finale
        if (finalTitleCanvasGroup != null && !isEnding)
        {
            // Fade in Big Game Title
            float elapsed = 0f;
            while (elapsed < finalTitleFadeInDuration && !isEnding)
            {
                elapsed += Time.deltaTime;
                finalTitleCanvasGroup.alpha = Mathf.Clamp01(elapsed / finalTitleFadeInDuration);
                yield return null;
            }
            finalTitleCanvasGroup.alpha = 1f;

            // Hold on screen
            float waitTimer = 0f;
            while (waitTimer < finalTitleDisplayDuration && !isEnding)
            {
                waitTimer += Time.deltaTime;
                yield return null;
            }

            // Fade out Big Game Title
            elapsed = 0f;
            while (elapsed < finalTitleFadeOutDuration && !isEnding)
            {
                elapsed += Time.deltaTime;
                finalTitleCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / finalTitleFadeOutDuration);
                yield return null;
            }
            finalTitleCanvasGroup.alpha = 0f;
        }

        // 9. Return to MainMenu
        if (autoLoadNextScene && !isEnding)
        {
            isEnding = true;
            Debug.Log($"[CreditsController] Credits complete. Transitioning to '{nextSceneName}'");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void SkipCredits()
    {
        if (isEnding) return;
        Debug.Log("[CreditsController] Skipping credits.");
        isScrolling = false;
        isEnding = true;
        StopAllCoroutines();
        StartCoroutine(DirectFadeOutAndTransition());
    }

    private IEnumerator DirectFadeOutAndTransition()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        float creditsAlpha = contentCanvasGroup != null ? contentCanvasGroup.alpha : 0f;
        float titleAlpha = finalTitleCanvasGroup != null ? finalTitleCanvasGroup.alpha : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            if (contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(creditsAlpha, 0f, t);
            if (finalTitleCanvasGroup != null) finalTitleCanvasGroup.alpha = Mathf.Lerp(titleAlpha, 0f, t);
            yield return null;
        }

        if (autoLoadNextScene && !string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"[CreditsController] Loading scene: '{nextSceneName}'");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private CreditsData LoadCreditsData()
    {
        string jsonContent = string.Empty;

        if (creditsJsonFile != null && !string.IsNullOrEmpty(creditsJsonFile.text))
        {
            jsonContent = creditsJsonFile.text;
        }
        else if (File.Exists(jsonFilePath))
        {
            jsonContent = File.ReadAllText(jsonFilePath);
        }
        else
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, "credits.json");
            if (File.Exists(streamingPath))
            {
                jsonContent = File.ReadAllText(streamingPath);
            }
        }

        if (!string.IsNullOrEmpty(jsonContent))
        {
            try
            {
                return JsonUtility.FromJson<CreditsData>(jsonContent);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[CreditsController] Error parsing JSON: {ex.Message}");
            }
        }

        return null;
    }

    private string BuildFormattedCreditsString(CreditsData data)
    {
        StringBuilder sb = new StringBuilder();

        // Game Title & Subtitle in scrolling text
        if (!string.IsNullOrEmpty(data.gameTitle))
        {
            sb.AppendLine($"<size=150%><color={titleColorHex}><b>{data.gameTitle}</b></color></size>");
            if (!string.IsNullOrEmpty(data.gameSubtitle))
            {
                sb.AppendLine($"<size=85%><color={subtitleColorHex}>{data.gameSubtitle}</color></size>");
            }
            sb.AppendLine("\n\n");
        }

        // Sections
        if (data.sections != null)
        {
            foreach (var section in data.sections)
            {
                if (section == null) continue;

                if (!string.IsNullOrEmpty(section.header))
                {
                    sb.AppendLine($"<size=120%><color={sectionHeaderColorHex}><b>— {section.header} —</b></color></size>\n");
                }

                if (section.roles != null)
                {
                    foreach (var role in section.roles)
                    {
                        if (role == null) continue;

                        if (!string.IsNullOrEmpty(role.roleTitle))
                        {
                            sb.AppendLine($"<size=85%><color={roleTitleColorHex}>{role.roleTitle}</color></size>");
                        }

                        if (role.names != null)
                        {
                            foreach (var name in role.names)
                            {
                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    sb.AppendLine($"<size=105%><color={namesColorHex}><b>{name}</b></color></size>");
                                }
                            }
                        }
                        sb.AppendLine();
                    }
                }
                sb.AppendLine("\n");
            }
        }

        // Footer / Thank You
        if (!string.IsNullOrEmpty(data.footer))
        {
            sb.AppendLine("\n\n");
            sb.AppendLine($"<size=135%><color={footerColorHex}><b>{data.footer}</b></color></size>");
        }

        return sb.ToString();
    }

    private CreditsData GetFallbackCreditsData()
    {
        CreditsData fallback = new CreditsData();
        fallback.gameTitle = "CUTE RESTAURANT SIMULATOR";
        fallback.gameSubtitle = "A Culinary & Dungeon Hunting Adventure";
        fallback.sections = new List<CreditSection>
        {
            new CreditSection
            {
                header = "PROJECT LEADERSHIP & INTEGRATION",
                roles = new List<CreditRole>
                {
                    new CreditRole { roleTitle = "Game Director & Integration Lead", names = new List<string> { "Pitak Patumwan (Ohm)" } }
                }
            },
            new CreditSection
            {
                header = "DUNGEON & COMBAT SYSTEMS",
                roles = new List<CreditRole>
                {
                    new CreditRole { roleTitle = "Lead Dungeon & Combat Designer", names = new List<string> { "Suppanut Posiri (sara-r)" } }
                }
            },
            new CreditSection
            {
                header = "RESTAURANT & CULINARY SYSTEMS",
                roles = new List<CreditRole>
                {
                    new CreditRole { roleTitle = "Lead Restaurant & Culinary Designer", names = new List<string> { "Saranpoom S. Saranpoom" } }
                }
            }
        };
        fallback.footer = "THANK YOU FOR PLAYING!";
        return fallback;
    }
}

[System.Serializable]
public class CreditsData
{
    public string gameTitle;
    public string gameSubtitle;
    public List<CreditSection> sections = new List<CreditSection>();
    public string footer;
}

[System.Serializable]
public class CreditSection
{
    public string header;
    public List<CreditRole> roles = new List<CreditRole>();
}

[System.Serializable]
public class CreditRole
{
    public string roleTitle;
    public List<string> names = new List<string>();
}
