using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float waveDuration = 30f;

    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private BattlePanelManager battlePanelManager;

    [SerializeField] private MonsterLibrary monsterLibrary;
    public bool IsWaveRunning { get; private set; }

    private float timer;
    private int enemiesKilled = 0;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Update()
    {
        if (!IsWaveRunning) return;

        timer -= Time.deltaTime;
        battlePanelManager.UpdateTimer(timer);

        if (timer <= 0f || !HasLivingPlayerMonsters())
        {
            EndWave();
        }
    }

    public void ResetTimer()
    {
        timer = waveDuration;
        IsWaveRunning = false;
        battlePanelManager.UpdateTimer(timer);
    }

    public void StartWave()
    {
        Debug.Log("WaveManager: Starting wave!");
        IsWaveRunning = true;
        timer = waveDuration;
        battlePanelManager.UpdateTimer(timer);
        SpawnEnemyWave();
    }

    private bool HasLivingPlayerMonsters()
    {
        return MonsterSpawner.Instance.GetPlayerMonsters().Any(m => m.IsAlive());
    }


    private void SpawnEnemyWave()
    {
        Debug.Log("WaveManager: Spawning enemy wave...");

        // This is where you'd decide which enemies appear.
        // For now, let's simulate 3 random enemy monsters:

        for (int i = 0; i < 3; i++)
        {
            MonsterDataSO enemyMonster = monsterLibrary.GetRandomMonster();
            monsterSpawner.SpawnEnemyMonster(enemyMonster);
        }
    }

    public void EndWave()
    {
        Debug.Log("WaveManager: Wave time over!");

        IsWaveRunning = false;

        int coinsEarned = RewardSystem.Instance.CalculateWaveRewards(enemiesKilled);
        enemiesKilled = 0;

        BattleManager.Instance.EnterResultsPhase(coinsEarned);
        
        if (!HasLivingPlayerMonsters())
        {
            BattleManager.Instance.EnterGameOver(RewardSystem.Instance.TotalCoinsEarned);
            return;
        }
    }


    public void RegisterEnemyKill()
    {
        enemiesKilled++;
        Debug.Log($"WaveManager: Enemies killed this wave = {enemiesKilled}");
    }

    public int GetEnemiesKilled()
    {
        return enemiesKilled;
    }

}
