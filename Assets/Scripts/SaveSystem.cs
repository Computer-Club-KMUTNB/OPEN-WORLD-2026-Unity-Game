using System.IO;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public int playerMoney = 0;
    public int rawBeefStock = 0;
    public int rawPorkStock = 0;
    public int rawRiceStock = 5;
    public int rawVeggieStock = 5;
    public int dayNumber = 1;
    public string lastSavedTimestamp = "";
}

public static class SaveSystem
{
    private static bool isInitialized = false;

    public static string GetSavePath()
    {
        string dir = Path.Combine(Application.dataPath, "Data");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return Path.Combine(dir, "savegame.json");
    }

    public static bool HasSaveFile()
    {
        string path = GetSavePath();
        return File.Exists(path);
    }

    public static void StartNewGame()
    {
        GameManager.globalMoney = 0;
        GameManager.globalBeef = 0;
        GameManager.globalPork = 0;
        GameManager.globalRice = 5;
        GameManager.globalVeggie = 5;
        SummaryDataBridge.globalDayNumber = 1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMoneyText();
        }

        Save();
        isInitialized = true;
        Debug.Log("✨ Started New Game: Money=0, Beef=0, Pork=0, Rice=5, Veggie=5");
    }

    public static void Save()
    {
        GameSaveData data = new GameSaveData
        {
            playerMoney = GameManager.globalMoney,
            rawBeefStock = GameManager.globalBeef,
            rawPorkStock = GameManager.globalPork,
            rawRiceStock = GameManager.globalRice,
            rawVeggieStock = GameManager.globalVeggie,
            dayNumber = (SummaryDataBridge.Instance != null) ? SummaryDataBridge.Instance.dayNumber : 1,
            lastSavedTimestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        string json = JsonUtility.ToJson(data, true);
        string path = GetSavePath();
        File.WriteAllText(path, json);
        Debug.Log($"💾 Game saved to JSON: {path}\nMoney: {data.playerMoney}G | Beef: {data.rawBeefStock} | Pork: {data.rawPorkStock} | Rice: {data.rawRiceStock} | Veggie: {data.rawVeggieStock}");
    }

    public static bool Load()
    {
        string path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.Log("ℹ️ No previous save file found in Assets/Data/. Using default initial stats.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            if (data != null)
            {
                GameManager.globalMoney = data.playerMoney;
                GameManager.globalBeef = data.rawBeefStock;
                GameManager.globalPork = data.rawPorkStock;
                GameManager.globalRice = data.rawRiceStock;
                GameManager.globalVeggie = data.rawVeggieStock;

                if (SummaryDataBridge.Instance != null)
                {
                    SummaryDataBridge.Instance.dayNumber = Mathf.Max(1, data.dayNumber);
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.UpdateMoneyText();
                }

                isInitialized = true;
                Debug.Log($"📂 Game loaded from JSON: {path} (Money: {data.playerMoney}G, Beef: {data.rawBeefStock}, Pork: {data.rawPorkStock}, Rice: {data.rawRiceStock}, Veggie: {data.rawVeggieStock})");
                return true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error loading save file from JSON: " + ex.Message);
        }

        return false;
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
