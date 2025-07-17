using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Global Timer Settings")]
    [SerializeField] private float totalGameTime = 300f; // 5 minutes

    [Header("References")]
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private BattlePanelManager battlePanelManager;

    [SerializeField] private MonsterLibrary monsterLibrary;
    public bool IsWaveRunning { get; private set; }
    private bool isTimerRunning = false;
    private float timeRemaining;
    private int enemiesKilled = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        timeRemaining = totalGameTime;
        battlePanelManager.UpdateTimer(timeRemaining);
    }


    private void Update()
    {
        if (!isTimerRunning) return;

        timeRemaining -= Time.deltaTime;
        battlePanelManager.UpdateTimer(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isTimerRunning = false;
            EndGameDueToTime();
            return;
        }

        if (IsWaveRunning && !HasLivingEnemyMonsters())
        {
            EndWave();
        }
    }

    public void StartWave()
    {
        Debug.Log("WaveManager: Starting wave!");
        IsWaveRunning = true;
        isTimerRunning = true;

        ClearAllMonstersStatusEffects();
        SpawnEnemyWave();
    }


    private void SpawnEnemyWave()
    {
        for (int i = 0; i < 3; i++)
        {
            MonsterDataSO enemyMonster = monsterLibrary.GetRandomMonster();
            monsterSpawner.SpawnEnemyMonster(enemyMonster);
        }
    }

    private void EndGameDueToTime()
    {
        Debug.Log("WaveManager: Time’s up!");
        IsWaveRunning = false;

        BattleManager.Instance.EnterGameOver(RewardSystem.Instance.TotalCoinsEarned);
    }

    public void EndWave()
    {
        Debug.Log("WaveManager: Enemy team defeated!");

        IsWaveRunning = false;
        PauseGlobalTimer();

        int coinsEarned = RewardSystem.Instance.CalculateWaveRewards(enemiesKilled);
        enemiesKilled = 0;

        BattleManager.Instance.EnterResultsPhase(coinsEarned);

        if (!HasLivingPlayerMonsters())
        {
            BattleManager.Instance.EnterGameOver(RewardSystem.Instance.TotalCoinsEarned);
        }
    }

    private void ClearAllMonstersStatusEffects()
    {
        foreach (var monster in MonsterSpawner.Instance.GetPlayerMonsters())
            monster.ClearStatusEffect();

        foreach (var monster in MonsterSpawner.Instance.GetEnemyMonsters())
            monster.ClearStatusEffect();
    }

    public void RegisterEnemyKill() => enemiesKilled++;
    public int GetEnemiesKilled() => enemiesKilled;
    public void PauseGlobalTimer() => isTimerRunning = false;
    public void ResumeGlobalTimer() => isTimerRunning = true;
    public float GetTimeRemaining() => timeRemaining;
    private bool HasLivingEnemyMonsters() => MonsterSpawner.Instance.GetEnemyMonsters().Any(m => m.IsAlive());
    private bool HasLivingPlayerMonsters() => MonsterSpawner.Instance.GetPlayerMonsters().Any(m => m.IsAlive());

}
