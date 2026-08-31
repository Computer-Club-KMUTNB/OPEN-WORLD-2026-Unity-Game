using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShiftSummaryController : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI dayTitleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Service Performance Stats")]
    [SerializeField] private TextMeshProUGUI happyGuestsText;
    [SerializeField] private TextMeshProUGUI dishesCookedText;
    [SerializeField] private List<Image> ratingStarImages = new List<Image>();
    [SerializeField] private Sprite goldStarSprite;
    [SerializeField] private Sprite emptyStarSprite;
    [SerializeField] private TextMeshProUGUI ratingLabelText;

    [Header("Financial Breakdown Card")]
    [SerializeField] private GameObject financialCardPanel;
    [SerializeField] private TextMeshProUGUI grossSalesText;
    [SerializeField] private TextMeshProUGUI kitchenUpkeepText;
    [SerializeField] private TextMeshProUGUI customerTipsText;
    [SerializeField] private TextMeshProUGUI netProfitText;

    [Header("Action Buttons")]
    [SerializeField] private GameObject actionButtonsPanel;
    [SerializeField] private Button okButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Navigation")]
    [SerializeField] private string restaurantSceneName = "Dev_Restaurant_Flow";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (happyGuestsText != null) happyGuestsText.text = "";
        if (dishesCookedText != null) dishesCookedText.text = "";
        if (ratingLabelText != null) ratingLabelText.text = "";

        if (grossSalesText != null) grossSalesText.text = "";
        if (kitchenUpkeepText != null) kitchenUpkeepText.text = "";
        if (customerTipsText != null) customerTipsText.text = "";
        if (netProfitText != null) netProfitText.text = "";

        if (financialCardPanel != null) financialCardPanel.SetActive(false);
        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);

        for (int i = 0; i < ratingStarImages.Count; i++)
        {
            if (ratingStarImages[i] != null && emptyStarSprite != null)
            {
                ratingStarImages[i].sprite = emptyStarSprite;
                ratingStarImages[i].transform.localScale = Vector3.zero;
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PlayShiftSequence());
    }

    private IEnumerator PlayShiftSequence()
    {
        SummaryDataBridge bridge = SummaryDataBridge.Instance;
        if (bridge == null)
        {
            GameObject go = new GameObject("SummaryDataBridge");
            bridge = go.AddComponent<SummaryDataBridge>();
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 1. Day Title Pop
        if (dayTitleText != null)
        {
            dayTitleText.text = $"<b>DAY {bridge.dayNumber} COMPLETED!</b>";
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(dayTitleText.transform, Vector3.one, 1.25f, 0.25f));
        }

        yield return new WaitForSecondsRealtime(0.15f);

        // 2. Roll Guests Served
        if (happyGuestsText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                happyGuestsText,
                bridge.happyGuests,
                prefix: "",
                suffix: $" / {bridge.totalGuests} Happy Guests",
                duration: 0.75f,
                maxRandomNumber: 30
            ));
        }

        // 3. Roll Dishes Cooked
        if (dishesCookedText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                dishesCookedText,
                bridge.dishesServed,
                prefix: "",
                suffix: " Dishes Cooked",
                duration: 0.75f,
                maxRandomNumber: 25
            ));
        }

        yield return new WaitForSecondsRealtime(0.15f);

        // 4. Pop Rating Stars sequentially
        int fullStars = Mathf.RoundToInt(bridge.starRating);
        for (int i = 0; i < ratingStarImages.Count; i++)
        {
            if (ratingStarImages[i] != null)
            {
                if (i < fullStars && goldStarSprite != null)
                {
                    ratingStarImages[i].sprite = goldStarSprite;
                }
                yield return StartCoroutine(SuspenseStatRoller.RevealCardWithPunch(ratingStarImages[i].gameObject, 0.18f));
                yield return new WaitForSecondsRealtime(0.06f);
            }
        }

        if (ratingLabelText != null)
        {
            string rankTitle;
            if (bridge.starRating >= 4.75f) rankTitle = "Legendary 5-Star Chef!";
            else if (bridge.starRating >= 3.8f) rankTitle = "Great Cooking & Service!";
            else if (bridge.starRating >= 2.8f) rankTitle = "Good Shift!";
            else if (bridge.starRating >= 1.8f) rankTitle = "Needs More Practice";
            else rankTitle = "Tough Day at Shop";

            ratingLabelText.text = $"{bridge.starRating:F1} ({rankTitle})";
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(ratingLabelText.transform, Vector3.one, 1.15f, 0.18f));
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 5. Reveal Financial Card
        if (financialCardPanel != null)
        {
            financialCardPanel.SetActive(true);
            yield return StartCoroutine(SuspenseStatRoller.RevealCardWithPunch(financialCardPanel, 0.25f));
        }

        // Top Row: Gross Sales
        if (grossSalesText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                grossSalesText,
                bridge.grossRevenue,
                prefix: "Gross Meal Sales: <color=#2E8B57>+",
                suffix: " G</color>",
                duration: 0.65f,
                maxRandomNumber: 4000
            ));
        }

        yield return new WaitForSecondsRealtime(0.1f);

        // Second Row: Upkeep
        if (kitchenUpkeepText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                kitchenUpkeepText,
                bridge.kitchenUpkeep,
                prefix: "Kitchen Upkeep & Spices: <color=#D9381E>-",
                suffix: " G</color>",
                duration: 0.6f,
                maxRandomNumber: 800
            ));
        }

        yield return new WaitForSecondsRealtime(0.1f);

        // Third Row: Tips
        if (customerTipsText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                customerTipsText,
                bridge.customerTips,
                prefix: "Generous Tips: <color=#DAA520>+",
                suffix: " G</color>",
                duration: 0.6f,
                maxRandomNumber: 500
            ));
        }

        yield return new WaitForSecondsRealtime(0.15f);

        // Net Profit
        if (netProfitText != null)
        {
            int net = bridge.netProfit;
            string colorHex = net >= 0 ? "#2E8B57" : "#D9381E";
            string sign = net >= 0 ? "+" : "";

            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                netProfitText,
                net,
                prefix: $"Net Profit: <color={colorHex}>{sign}",
                suffix: " G</color>",
                duration: 0.8f,
                maxRandomNumber: 5000
            ));

            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(netProfitText.transform, Vector3.one, 1.25f, 0.25f));
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 6. Reveal Action Buttons
        if (actionButtonsPanel != null)
        {
            actionButtonsPanel.SetActive(true);
            yield return StartCoroutine(SuspenseStatRoller.RevealCardWithPunch(actionButtonsPanel, 0.2f));
        }

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(OnOkClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }

    public void OnOkClicked()
    {
        Time.timeScale = 1.0f;
        string target = restaurantSceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "restaurant-scene";
        }
        SceneManager.LoadScene(target);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
