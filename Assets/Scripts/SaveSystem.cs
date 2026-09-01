using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int playerMoney = 0;
    public int rawBeefStock = 0;
    public int rawPorkStock = 0;
    public int rawRiceStock = 10;
    public int rawVeggieStock = 10;
    public int dayNumber = 1;
    public string lastSavedTimestamp = "";
}

public static class SaveSystem
{
    private static bool isInitialized = false;
    private const string SAVE_FILE_NAME = "savegame.dat";
    private const int SAVE_MAGIC = 0x53415645; // "SAVE" in ASCII header
    private const int SAVE_VERSION = 1;

    /// <summary>
    /// Returns the standard persistent save path (LocalLow on Windows):
    /// C:\Users\<Username>\AppData\LocalLow\<CompanyName>\<ProductName>\savegame.dat
    /// </summary>
    public static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }

    /// <summary>
    /// Legacy JSON paths for auto-migration
    /// </summary>
    private static string GetLegacyJsonPersistentPath() => Path.Combine(Application.persistentDataPath, "savegame.json");
    private static string GetLegacyJsonDataPath() => Path.Combine(Application.dataPath, "Data", "savegame.json");

    public static bool HasSaveFile()
    {
        if (File.Exists(GetSavePath())) return true;
        if (File.Exists(GetLegacyJsonPersistentPath())) return true;
        if (File.Exists(GetLegacyJsonDataPath())) return true;
        return false;
    }

    public static void StartNewGame()
    {
        GameManager.globalMoney = 0;
        GameManager.globalBeef = 0;
        GameManager.globalPork = 0;
        GameManager.globalRice = 10;
        GameManager.globalVeggie = 10;
        SummaryDataBridge.globalDayNumber = 1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMoneyText();
        }

        Save();
        isInitialized = true;
        Debug.Log("✨ Started New Game: Money=0, Beef=0, Pork=0, Rice=5, Veggie=5 (Saved to LocalLow savegame.dat)");
    }

    public static void Save()
    {
        try
        {
            GameSaveData data = new GameSaveData
            {
                playerMoney = GameManager.globalMoney,
                rawBeefStock = GameManager.globalBeef,
                rawPorkStock = GameManager.globalPork,
                rawRiceStock = GameManager.globalRice,
                rawVeggieStock = GameManager.globalVeggie,
                dayNumber = (SummaryDataBridge.Instance != null) ? SummaryDataBridge.Instance.dayNumber : SummaryDataBridge.globalDayNumber,
                lastSavedTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string primaryPath = GetSavePath();
            string dir = Path.GetDirectoryName(primaryPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // Atomic binary write via temp file
            string tempPath = primaryPath + ".tmp";
            using (FileStream stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(SAVE_MAGIC);
                writer.Write(SAVE_VERSION);
                writer.Write(data.playerMoney);
                writer.Write(data.rawBeefStock);
                writer.Write(data.rawPorkStock);
                writer.Write(data.rawRiceStock);
                writer.Write(data.rawVeggieStock);
                writer.Write(data.dayNumber);
                writer.Write(data.lastSavedTimestamp);
            }

            if (File.Exists(primaryPath))
            {
                File.Delete(primaryPath);
            }
            File.Move(tempPath, primaryPath);

            Debug.Log($"💾 Game saved to LocalLow binary ({primaryPath}): Money={data.playerMoney}G | Beef={data.rawBeefStock} | Pork={data.rawPorkStock} | Rice={data.rawRiceStock} | Veggie={data.rawVeggieStock} | Day={data.dayNumber}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to save binary game data to LocalLow: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static bool Load()
    {
        string primaryPath = GetSavePath();

        // 1. Try reading binary savegame.dat in LocalLow
        if (File.Exists(primaryPath))
        {
            try
            {
                using (FileStream stream = new FileStream(primaryPath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int magic = reader.ReadInt32();
                    if (magic == SAVE_MAGIC)
                    {
                        int version = reader.ReadInt32();
                        int money = reader.ReadInt32();
                        int beef = reader.ReadInt32();
                        int pork = reader.ReadInt32();
                        int rice = reader.ReadInt32();
                        int veggie = reader.ReadInt32();
                        int day = reader.ReadInt32();
                        string timestamp = reader.ReadString();

                        ApplyLoadedData(money, beef, pork, rice, veggie, day);
                        isInitialized = true;
                        Debug.Log($"📂 Game loaded from LocalLow binary '{primaryPath}' (Day {day}, Money: {money}G, Beef: {beef}, Pork: {pork}, Rice: {rice}, Veggie: {veggie})");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"⚠️ Failed to read binary save in LocalLow: {ex.Message}. Checking legacy fallback...");
            }
        }

        // 2. Fallback / Auto-Migration: If legacy JSON file exists, migrate it into savegame.dat in LocalLow
        string legacyPersistent = GetLegacyJsonPersistentPath();
        string legacyData = GetLegacyJsonDataPath();
        string jsonPathToMigrate = File.Exists(legacyPersistent) ? legacyPersistent : (File.Exists(legacyData) ? legacyData : null);

        if (jsonPathToMigrate != null)
        {
            try
            {
                string json = File.ReadAllText(jsonPathToMigrate);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                if (data != null)
                {
                    ApplyLoadedData(data.playerMoney, data.rawBeefStock, data.rawPorkStock, data.rawRiceStock, data.rawVeggieStock, data.dayNumber);
                    isInitialized = true;
                    // Convert and save immediately to LocalLow binary savegame.dat
                    Save();
                    Debug.Log($"🔄 Successfully migrated legacy JSON '{jsonPathToMigrate}' to LocalLow binary '{primaryPath}'");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error during legacy JSON migration: {ex.Message}");
            }
        }

        Debug.Log($"ℹ️ No save file found in LocalLow ({primaryPath}). Using default starting stats.");
        return false;
    }

    private static void ApplyLoadedData(int money, int beef, int pork, int rice, int veggie, int day)
    {
        GameManager.globalMoney = money;
        GameManager.globalBeef = beef;
        GameManager.globalPork = pork;
        GameManager.globalRice = rice;
        GameManager.globalVeggie = veggie;
        SummaryDataBridge.globalDayNumber = Mathf.Max(1, day);

        if (SummaryDataBridge.Instance != null)
        {
            SummaryDataBridge.Instance.dayNumber = Mathf.Max(1, day);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMoneyText();
        }
    }

    public static void DeleteSave()
    {
        try
        {
            string path = GetSavePath();
            if (File.Exists(path)) File.Delete(path);

            string legacy1 = GetLegacyJsonPersistentPath();
            if (File.Exists(legacy1)) File.Delete(legacy1);

            string legacy2 = GetLegacyJsonDataPath();
            if (File.Exists(legacy2)) File.Delete(legacy2);

            Debug.Log("🗑️ All save files cleared from LocalLow.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error deleting save file: {ex.Message}");
        }
    }

    public static void InitializeOnStartup()
    {
        if (!isInitialized)
        {
            Load();
            isInitialized = true;
        }
    }
}
