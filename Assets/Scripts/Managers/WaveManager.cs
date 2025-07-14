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

    private float timer;
    private bool isWaveRunning = false;
    private int enemiesKilled = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (!isWaveRunning) return;

        timer -= Time.deltaTime;
        battlePanelManager.UpdateTimer(timer);

        if (timer <= 0f)
        {
            EndWave();
        }
    }

    public void ResetTimer()
    {
        timer = waveDuration;
        isWaveRunning = false;
        battlePanelManager.UpdateTimer(timer);
    }

    public void StartWave()
    {
        Debug.Log("WaveManager: Starting wave!");
        isWaveRunning = true;
        timer = waveDuration;
        battlePanelManager.UpdateTimer(timer);

        SpawnEnemyWave();
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

    private void EndWave()
    {
        Debug.Log("WaveManager: Wave time over!");

        isWaveRunning = false;

        int coinsEarned = RewardSystem.Instance.CalculateWaveRewards(enemiesKilled);
        enemiesKilled = 0; // reset for next wave

        BattleManager.Instance.EnterResultsPhase(coinsEarned);
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
