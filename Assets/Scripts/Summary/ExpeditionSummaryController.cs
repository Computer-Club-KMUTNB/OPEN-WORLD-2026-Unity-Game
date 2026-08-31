using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExpeditionSummaryController : MonoBehaviour
{
    [Header("Title & Rank")]
    [SerializeField] private TextMeshProUGUI headerTitleText;
    [SerializeField] private TextMeshProUGUI rankBadgeText;
    [SerializeField] private TextMeshProUGUI rankSubtitleText;

    [Header("Stats Displays")]
    [SerializeField] private TextMeshProUGUI beastsSlainText;
    [SerializeField] private TextMeshProUGUI beastsBreakdownText;
    [SerializeField] private TextMeshProUGUI timeInWildText;
    [SerializeField] private TextMeshProUGUI damageDealtText;

    [Header("Loot Grid")]
    [SerializeField] private GameObject lootSectionPanel;
    [SerializeField] private List<GameObject> lootCardObjects = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> lootNameTexts = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> lootQtyTexts = new List<TextMeshProUGUI>();

    [Header("Action Buttons Panel")]
    [SerializeField] private GameObject actionButtonsPanel;
    [SerializeField] private Button backToRestaurantButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scene Navigation")]
    [SerializeField] private string restaurantSceneName = "restaurant-scene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        // Start completely blank so nothing shows instantly before animation
        if (rankBadgeText != null) rankBadgeText.text = "";
        if (rankSubtitleText != null) rankSubtitleText.text = "";
        if (beastsSlainText != null) beastsSlainText.text = "";
        if (beastsBreakdownText != null) beastsBreakdownText.text = "";
        if (timeInWildText != null) timeInWildText.text = "";
        if (damageDealtText != null) damageDealtText.text = "";

        if (lootSectionPanel != null) lootSectionPanel.SetActive(false);
        for (int i = 0; i < lootCardObjects.Count; i++)
        {
            if (lootCardObjects[i] != null) lootCardObjects[i].SetActive(false);
        }

        if (actionButtonsPanel != null) actionButtonsPanel.SetActive(false);
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PlaySummarySequence());
    }

    private IEnumerator PlaySummarySequence()
    {
        SummaryDataBridge bridge = SummaryDataBridge.Instance;
        if (bridge == null)
        {
            GameObject go = new GameObject("SummaryDataBridge");
            bridge = go.AddComponent<SummaryDataBridge>();
            bridge.PopulateDefaultDemoDataIfEmpty();
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 1. Roll Threat Rank with suspense (Blank -> Animation -> Reveal)
        string[] fakeRanks = new string[] { "D-RANK", "C-RANK", "B-RANK", "A-RANK", "SS-RANK", "S-RANK" };
        if (rankBadgeText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollTextChoicesCoroutine(
                rankBadgeText,
                fakeRanks,
                $"<b>{bridge.threatRank}</b>",
                duration: 1.1f
            ));
        }

        if (rankSubtitleText != null)
        {
            rankSubtitleText.text = bridge.threatSubtitle;
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(rankSubtitleText.transform, Vector3.one, 1.25f, 0.2f));
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // 2. Roll Beasts Slain (Blank -> Animation -> Reveal)
        if (beastsSlainText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                beastsSlainText,
                bridge.totalBeastsSlain,
                prefix: "BEASTS SLAIN: ",
                suffix: " KILLS",
                duration: 0.8f,
                maxRandomNumber: 25
            ));
        }

        if (beastsBreakdownText != null)
        {
            beastsBreakdownText.text = bridge.beastsBreakdown;
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(beastsBreakdownText.transform, Vector3.one, 1.15f, 0.15f));
        }

        yield return new WaitForSecondsRealtime(0.15f);

        // 3. Reveal Time & Damage (Blank -> Animation -> Reveal)
        if (timeInWildText != null)
        {
            timeInWildText.text = $"TIME: {bridge.timeInWild}";
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(timeInWildText.transform, Vector3.one, 1.2f, 0.18f));
        }

        if (damageDealtText != null)
        {
            yield return StartCoroutine(SuspenseStatRoller.RollNumberCoroutine(
                damageDealtText,
                bridge.totalDamageDealt,
                prefix: "",
                suffix: " DMG DEALT",
                duration: 0.8f,
                maxRandomNumber: 20000
            ));
        }

        yield return new WaitForSecondsRealtime(0.25f);

        // 4. Reveal Harvested Loot Cards sequentially (Blank -> Punch in one by one)
        if (lootSectionPanel != null)
        {
            lootSectionPanel.SetActive(true);
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(lootSectionPanel.transform, Vector3.one, 1.05f, 0.15f));
        }

        for (int i = 0; i < lootCardObjects.Count; i++)
        {
            if (i < bridge.harvestedLoot.Count && lootCardObjects[i] != null)
            {
                var loot = bridge.harvestedLoot[i];
                if (i < lootNameTexts.Count && lootNameTexts[i] != null) lootNameTexts[i].text = loot.itemName;
                if (i < lootQtyTexts.Count && lootQtyTexts[i] != null) lootQtyTexts[i].text = $"x{loot.quantity}";

                yield return StartCoroutine(SuspenseStatRoller.RevealCardWithPunch(lootCardObjects[i], duration: 0.22f));
                yield return new WaitForSecondsRealtime(0.08f);
            }
        }

        yield return new WaitForSecondsRealtime(0.25f);

        // 5. Reveal Action Buttons
        if (actionButtonsPanel != null)
        {
            actionButtonsPanel.SetActive(true);
            yield return StartCoroutine(SuspenseStatRoller.BounceSlam(actionButtonsPanel.transform, Vector3.one, 1.15f, 0.2f));
        }
    }

    public void OnBackToRestaurantClicked()
    {
        if (SummaryDataBridge.Instance != null)
        {
            SummaryDataBridge.Instance.TransferLootToGameManager();
        }

        string target = restaurantSceneName;
        if (!Application.CanStreamedLevelBeLoaded(target))
        {
            target = "restaurant-scene";
        }
        SceneManager.LoadScene(target);
    }

    public void OnReturnToMainMenuClicked()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
