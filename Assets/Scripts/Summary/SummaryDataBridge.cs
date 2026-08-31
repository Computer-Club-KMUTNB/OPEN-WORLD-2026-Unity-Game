using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootReward
{
    public string itemName;
    public int quantity;
    public string iconName;
}

public class SummaryDataBridge : MonoBehaviour
{
    public static SummaryDataBridge Instance { get; private set; }

    [Header("Expedition Hunt Results")]
    public string threatRank = "S-RANK";
    public string threatSubtitle = "BRUTAL VICTORY";
    public int totalBeastsSlain = 8;
    public string beastsBreakdown = "3x Dire Boars, 3x Venom Crawlers, 2x Shadow Drakes";
    public string timeInWild = "04:25";
    public int totalDamageDealt = 14250;
    public List<LootReward> harvestedLoot = new List<LootReward>();

    [Header("Restaurant Shift Results")]
    public int dayNumber = 1;
    public int happyGuests = 18;
    public int totalGuests = 20;
    public float starRating = 5.0f;
    public int dishesServed = 18;
    public int grossRevenue = 2450;
    public int kitchenUpkeep = 350;
    public int customerTips = 300;
    public int netProfit => (grossRevenue - kitchenUpkeep + customerTips);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PopulateDefaultDemoDataIfEmpty();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PopulateDefaultDemoDataIfEmpty()
    {
        if (harvestedLoot == null || harvestedLoot.Count == 0)
        {
            harvestedLoot = new List<LootReward>
            {
                new LootReward { itemName = "Tender Dire Meat", quantity = 6, iconName = "meat" },
                new LootReward { itemName = "Iron Beast Ribs", quantity = 4, iconName = "bone" },
                new LootReward { itemName = "Drake Venom Sac", quantity = 2, iconName = "venom" },
                new LootReward { itemName = "Wild Savage Herbs", quantity = 5, iconName = "herb" }
            };
        }
    }
}
