using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private BattlePanelManager battlePanelManager;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private MonsterManager monsterManager;

    private bool isCombatActive = false;

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

    public void StartCombat()
    {
        Debug.Log("CombatSystem: Starting combat phase.");
        isCombatActive = true;

        // Clear any old monsters
        monsterSpawner.ClearAllMonsters();

        // Spawn player and enemy wave picks
        monsterSpawner.SpawnPlayerMonsters(new System.Collections.Generic.List<MonsterDataSO>(monsterManager.PlayerWavePicks));
        monsterSpawner.SpawnEnemyMonsters(new System.Collections.Generic.List<MonsterDataSO>(monsterManager.EnemyWavePicks));

        // Start checking for combat completion
        StartCoroutine(MonitorCombat());
    }

    private IEnumerator MonitorCombat()
    {
        Debug.Log("CombatSystem: Monitoring combat...");

        yield return new WaitForSeconds(1f); // optional delay before checking

        while (isCombatActive)
        {
            // Check if player has any monsters left
            bool playerHasMonsters = HasAnyPlayerMonsters();
            bool enemyHasMonsters = HasAnyEnemyMonsters();

            if (!playerHasMonsters || !enemyHasMonsters)
            {
                Debug.Log("CombatSystem: Combat over.");
                isCombatActive = false;
                EndCombat();
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private bool HasAnyPlayerMonsters()
    {
        foreach (var monster in FindObjectsOfType<MonsterController>())
        {
            if (monster.isPlayer) return true;
        }
        return false;
    }

    private bool HasAnyEnemyMonsters()
    {
        foreach (var monster in FindObjectsOfType<MonsterController>())
        {
            if (!monster.isPlayer) return true;
        }
        return false;
    }

    private void EndCombat()
    {
        Debug.Log("CombatSystem: Ending combat phase.");

        // Tell WaveManager to stop the timer
        WaveManager.Instance.ResetTimer();

        // Reward coins based on enemies killed
        // WaveManager handles counting enemiesKilled during battle

        int coinsEarned = RewardSystem.Instance.CalculateWaveRewards(WaveManager.Instance.GetEnemiesKilled());
        RewardSystem.Instance.GrantRewards(coinsEarned);

        // Notify BattleManager to show results
        BattleManager.Instance.EnterResultsPhase(coinsEarned);
    }
}
