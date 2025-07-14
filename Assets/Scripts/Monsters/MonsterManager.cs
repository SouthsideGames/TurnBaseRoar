using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }

    [SerializeField] private List<OwnedMonster> ownedMonsters = new List<OwnedMonster>();
    [SerializeField] private List<OwnedMonster> teamMonsters = new List<OwnedMonster>();

    public IReadOnlyList<OwnedMonster> OwnedMonsters => ownedMonsters;
    public IReadOnlyList<OwnedMonster> TeamMonsters => teamMonsters;

    private List<MonsterDataSO> playerWavePicks = new List<MonsterDataSO>();
    private List<MonsterDataSO> enemyWavePicks = new List<MonsterDataSO>();

    public IReadOnlyList<MonsterDataSO> PlayerWavePicks => playerWavePicks;
    public IReadOnlyList<MonsterDataSO> EnemyWavePicks => enemyWavePicks;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMonster(MonsterDataSO newMonsterData)
    {
        if (newMonsterData == null) return;

        ownedMonsters.Add(new OwnedMonster(newMonsterData));
        SaveData();
    }

    public void SetTeam(List<OwnedMonster> newTeam)
    {
        if (newTeam == null) return;

        teamMonsters = new List<OwnedMonster>(newTeam);
        SaveData();
    }

    public void LoadData()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager not initialized!");
            return;
        }

        ownedMonsters = SaveManager.Instance.LoadPlayerMonsters();
    }

    public void SaveData()
    {
        SaveManager.Instance.SavePlayerMonsters(ownedMonsters);
    }

    public void ClearAllData()
    {
        ownedMonsters.Clear();
        teamMonsters.Clear();
        PlayerPrefs.DeleteKey(SaveManager.PlayerCollectionKey);
        PlayerPrefs.Save();
    }

    public void AddPlayerDraftPick(MonsterDataSO pickedMonster)
    {
        if (pickedMonster == null)
        {
            Debug.LogWarning("MonsterManager: Player pick was null!");
            return;
        }

        playerWavePicks.Add(pickedMonster);
        Debug.Log($"MonsterManager: Added player pick {pickedMonster.monsterName}");
    }

    public void AddEnemyDraftPick(MonsterDataSO pickedMonster)
    {
        if (pickedMonster == null)
        {
            Debug.LogWarning("MonsterManager: Enemy pick was null!");
            return;
        }

        enemyWavePicks.Add(pickedMonster);
        Debug.Log($"MonsterManager: Added enemy pick {pickedMonster.monsterName}");
    }

    public void ClearWavePicks()
    {
        playerWavePicks.Clear();
        enemyWavePicks.Clear();
    }
}
