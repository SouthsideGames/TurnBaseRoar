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
    public bool PauseTimer { get; set; } = false;

    private float timer;
    private int enemiesKilled = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Update()
    {
        if (!IsWaveRunning || PauseTimer) return;

        timer -= Time.deltaTime;
        battlePanelManager.UpdateTimer(timer);

        if (timer <= 0f || !HasLivingPlayerMonsters())
            EndWave();
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
        ClearAllMonstersStatusEffects();
        SpawnEnemyWave();
    }

    private bool HasLivingPlayerMonsters() => MonsterSpawner.Instance.GetPlayerMonsters().Any(m => m.IsAlive());


    private void SpawnEnemyWave()
    {
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

    private void ClearAllMonstersStatusEffects()
    {
        foreach (var monster in MonsterSpawner.Instance.GetPlayerMonsters())
            monster.ClearStatusEffect();

        foreach (var monster in MonsterSpawner.Instance.GetEnemyMonsters())
            monster.ClearStatusEffect();
    }


    public void RegisterEnemyKill()
    {
        enemiesKilled++;
        Debug.Log($"WaveManager: Enemies killed this wave = {enemiesKilled}");
    }

    public int GetEnemiesKilled() => enemiesKilled;

}
