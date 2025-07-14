using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private MonsterLibrary monsterLibrary;

    public static string PlayerCollectionKey => "PlayerMonsterCollection";
    public static string PlayerCoinsKey => "PlayerCoins";

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SavePlayerMonsters(List<OwnedMonster> ownedMonsters)
    {
        if (ownedMonsters == null) return;

        List<OwnedMonsterSaveData> saveDataList = new List<OwnedMonsterSaveData>();
        foreach (var monster in ownedMonsters)
        {
            saveDataList.Add(new OwnedMonsterSaveData(monster));
        }

        string json = JsonUtility.ToJson(new OwnedMonsterSaveDataList(saveDataList));
        PlayerPrefs.SetString(PlayerCollectionKey, json);
        PlayerPrefs.Save();
    }

    public List<OwnedMonster> LoadPlayerMonsters()
    {
        if (!PlayerPrefs.HasKey(PlayerCollectionKey))
        {
            return new List<OwnedMonster>();
        }

        string json = PlayerPrefs.GetString(PlayerCollectionKey);
        var loadedList = JsonUtility.FromJson<OwnedMonsterSaveDataList>(json);

        List<OwnedMonster> result = new List<OwnedMonster>();
        foreach (var saveData in loadedList.monsterSaveData)
        {
            var monsterDataSO = monsterLibrary.GetMonsterByID(saveData.monsterID);
            if (monsterDataSO != null)
            {
                OwnedMonster owned = new OwnedMonster(monsterDataSO)
                {
                    level = saveData.level,
                    currentXP = saveData.currentXP
                };
                result.Add(owned);
            }
        }

        return result;
    }

    public int LoadCoins()
    {
        return PlayerPrefs.GetInt(PlayerCoinsKey, 0);
    }

    public void SaveCoins(int coins)
    {
        PlayerPrefs.SetInt(PlayerCoinsKey, coins);
        PlayerPrefs.Save();
    }


}
