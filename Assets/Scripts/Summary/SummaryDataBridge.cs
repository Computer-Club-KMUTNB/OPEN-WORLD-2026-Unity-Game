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
    public string beastsBreakdown = "3x Dire Beasts, 5x Shadow Monsters";
    public string timeInWild = "03:45";
    public int totalDamageDealt = 12500;
    public List<LootReward> harvestedLoot = new List<LootReward>();

    [Header("Harvested Meat Cache")]
    public int harvestedBeef = 0;
    public int harvestedPork = 0;

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
                new LootReward { itemName = "Raw Beef", quantity = Mathf.Max(harvestedBeef, 4), iconName = "meat" },
                new LootReward { itemName = "Raw Pork", quantity = Mathf.Max(harvestedPork, 3), iconName = "bone" },
                new LootReward { itemName = "Beast Bones", quantity = 2, iconName = "bone" },
                new LootReward { itemName = "Wild Herbs", quantity = 5, iconName = "herb" }
            };
        }
    }

    public void RecordHuntSession(int kills, int beefAmount, int porkAmount, float timeSeconds, int damage)
    {
        totalBeastsSlain = kills;
        harvestedBeef = beefAmount;
        harvestedPork = porkAmount;
        totalDamageDealt = damage;

        int mins = Mathf.FloorToInt(timeSeconds / 60f);
        int secs = Mathf.FloorToInt(timeSeconds % 60f);
        timeInWild = $"{mins:D2}:{secs:D2}";

        if (kills >= 10)
        {
            threatRank = "SS-RANK";
            threatSubtitle = "LEGENDARY SLAYER";
        }
        else if (kills >= 6)
        {
            threatRank = "S-RANK";
            threatSubtitle = "BRUTAL VICTORY";
        }
        else if (kills >= 3)
        {
            threatRank = "A-RANK";
            threatSubtitle = "SUCCESSFUL HUNT";
        }
        else
        {
            threatRank = "B-RANK";
            threatSubtitle = "SURVIVED";
        }

        beastsBreakdown = $"{kills}x Dungeon Monsters Slain";

        harvestedLoot.Clear();
        if (beefAmount > 0)
        {
            harvestedLoot.Add(new LootReward { itemName = "Raw Beef", quantity = beefAmount, iconName = "meat" });
        }
        if (porkAmount > 0)
        {
            harvestedLoot.Add(new LootReward { itemName = "Raw Pork", quantity = porkAmount, iconName = "bone" });
        }
        if (kills > 0)
        {
            harvestedLoot.Add(new LootReward { itemName = "Monster Bone", quantity = Mathf.Max(1, kills / 2), iconName = "bone" });
            harvestedLoot.Add(new LootReward { itemName = "Dungeon Herb", quantity = Mathf.Max(2, kills), iconName = "herb" });
        }

        if (harvestedLoot.Count == 0)
        {
            PopulateDefaultDemoDataIfEmpty();
        }
    }

    public void TransferLootToGameManager()
    {
        if (GameManager.Instance != null)
        {
            if (harvestedBeef > 0)
            {
                GameManager.Instance.AddHuntingLoot("RawBeef", harvestedBeef);
            }
            if (harvestedPork > 0)
            {
                GameManager.Instance.AddHuntingLoot("RawPork", harvestedPork);
            }

            Debug.Log($"Transferred Hunt Loot to Restaurant: Beef +{harvestedBeef}, Pork +{harvestedPork}");
            harvestedBeef = 0;
            harvestedPork = 0;
        }
    }

    public void RecordShiftSession(int happy, int total, int dishes, int revenue, int upkeep, int tips, float rating)
    {
        happyGuests = happy;
        totalGuests = total;
        dishesServed = dishes;
        grossRevenue = revenue;
        kitchenUpkeep = upkeep;
        customerTips = tips;
        starRating = rating;
    }
}
