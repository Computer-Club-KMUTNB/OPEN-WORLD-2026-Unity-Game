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

    // Static persistent fields (Guaranteed to retain real stats across scenes)
    public static string globalThreatRank = "B-RANK";
    public static string globalThreatSubtitle = "EXPEDITION COMPLETE";
    public static int globalBeastsSlain = 0;
    public static string globalBeastsBreakdown = "0x Monsters Slain";
    public static string globalTimeInWild = "00:00";
    public static int globalDamageDealt = 0;
    public static int globalHarvestedBeef = 0;
    public static int globalHarvestedPork = 0;
    public static List<LootReward> globalHarvestedLoot = new List<LootReward>();

    // Shift Stats
    public static int globalDayNumber = 1;
    public static int globalHappyGuests = 0;
    public static int globalTotalGuests = 0;
    public static float globalStarRating = 5.0f;
    public static int globalDishesServed = 0;
    public static int globalGrossRevenue = 0;
    public static int globalKitchenUpkeep = 0;
    public static int globalCustomerTips = 0;

    // Instance accessors for inspector / UI binding
    public string threatRank => globalThreatRank;
    public string threatSubtitle => globalThreatSubtitle;
    public int totalBeastsSlain => globalBeastsSlain;
    public string beastsBreakdown => globalBeastsBreakdown;
    public string timeInWild => globalTimeInWild;
    public int totalDamageDealt => globalDamageDealt;
    public List<LootReward> harvestedLoot => globalHarvestedLoot;

    public int harvestedBeef
    {
        get => globalHarvestedBeef;
        set => globalHarvestedBeef = value;
    }
    public int harvestedPork
    {
        get => globalHarvestedPork;
        set => globalHarvestedPork = value;
    }

    public int dayNumber
    {
        get => globalDayNumber;
        set => globalDayNumber = value;
    }
    public int happyGuests => globalHappyGuests;
    public int totalGuests => globalTotalGuests;
    public float starRating => globalStarRating;
    public int dishesServed => globalDishesServed;
    public int grossRevenue => globalGrossRevenue;
    public int kitchenUpkeep => globalKitchenUpkeep;
    public int customerTips => globalCustomerTips;
    public int netProfit => (globalGrossRevenue - globalKitchenUpkeep + globalCustomerTips);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PopulateDefaultDemoDataIfEmpty()
    {
        if (globalHarvestedLoot == null || globalHarvestedLoot.Count == 0)
        {
            globalHarvestedLoot = new List<LootReward>
            {
                new LootReward { itemName = "Raw Beef", quantity = globalHarvestedBeef, iconName = "meat" },
                new LootReward { itemName = "Raw Pork", quantity = globalHarvestedPork, iconName = "meat" }
            };
        }
    }

    public static void RecordHuntSession(int kills, int beefAmount, int porkAmount, float timeSeconds, int damage)
    {
        globalBeastsSlain = kills;
        globalHarvestedBeef = beefAmount;
        globalHarvestedPork = porkAmount;
        globalDamageDealt = damage;

        int mins = Mathf.FloorToInt(timeSeconds / 60f);
        int secs = Mathf.FloorToInt(timeSeconds % 60f);
        globalTimeInWild = $"{mins:D2}:{secs:D2}";

        if (kills >= 10)
        {
            globalThreatRank = "SS-RANK";
            globalThreatSubtitle = "LEGENDARY SLAYER";
        }
        else if (kills >= 6)
        {
            globalThreatRank = "S-RANK";
            globalThreatSubtitle = "BRUTAL VICTORY";
        }
        else if (kills >= 3)
        {
            globalThreatRank = "A-RANK";
            globalThreatSubtitle = "SUCCESSFUL HUNT";
        }
        else if (kills >= 1)
        {
            globalThreatRank = "B-RANK";
            globalThreatSubtitle = "EXPEDITION COMPLETE";
        }
        else
        {
            globalThreatRank = "C-RANK";
            globalThreatSubtitle = "RETREAT";
        }

        globalBeastsBreakdown = $"{kills}x Dungeon Monsters Slain";

        globalHarvestedLoot.Clear();
        globalHarvestedLoot.Add(new LootReward { itemName = "Raw Beef", quantity = beefAmount, iconName = "meat" });
        globalHarvestedLoot.Add(new LootReward { itemName = "Raw Pork", quantity = porkAmount, iconName = "meat" });

        Debug.Log($"[SummaryDataBridge] Recorded: Kills={kills}, Beef={beefAmount}, Pork={porkAmount}, Time={globalTimeInWild}");
    }

    public static void TransferLootToGameManager()
    {
        int beefToAdd = globalHarvestedBeef;
        int porkToAdd = globalHarvestedPork;

        if (beefToAdd > 0)
        {
            GameManager.globalBeef += beefToAdd;
            Debug.Log($"Transferred +{beefToAdd} Raw Beef into Pantry! Current Total Beef: {GameManager.globalBeef}");
        }
        if (porkToAdd > 0)
        {
            GameManager.globalPork += porkToAdd;
            Debug.Log($"Transferred +{porkToAdd} Raw Pork into Pantry! Current Total Pork: {GameManager.globalPork}");
        }

        globalHarvestedBeef = 0;
        globalHarvestedPork = 0;

        SaveSystem.Save();
    }

    public static void RecordShiftSession(int happy, int total, int dishes, int revenue, int upkeep, int tips, float rating)
    {
        globalHappyGuests = happy;
        globalTotalGuests = total;
        globalDishesServed = dishes;
        globalGrossRevenue = revenue;
        globalKitchenUpkeep = upkeep;
        globalCustomerTips = tips;
        globalStarRating = rating;
    }

    public static void ApplyShiftProfitToGameManager()
    {
        int profit = (globalGrossRevenue - globalKitchenUpkeep + globalCustomerTips);
        if (profit > 0)
        {
            GameManager.globalMoney += profit;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateMoneyText();
            }
            Debug.Log($"Added Net Profit +{profit} Gold to GameManager. Total Money: {GameManager.globalMoney}");
        }

        SaveSystem.Save();
    }
}
