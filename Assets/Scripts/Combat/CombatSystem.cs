using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private BattlePanelManager battlePanelManager;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private MonsterManager monsterManager;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartCombat()
    {
        Debug.Log("CombatSystem: Preparing battle...");

        // Clear old monsters
        monsterSpawner.ClearAllMonsters();

        // Spawn new monsters for this wave
        monsterSpawner.SpawnPlayerMonsters(monsterManager.PlayerWavePicks.ToList());
        monsterSpawner.SpawnEnemyMonsters(monsterManager.EnemyWavePicks.ToList());

        Debug.Log("CombatSystem: Spawns complete. Starting turn loop...");

        StartCoroutine(CombatRoutine());
    }


    private IEnumerator CombatRoutine()
    {
        Debug.Log("CombatSystem: Starting combat loop while wave is active.");

        while (WaveManager.Instance.IsWaveRunning)
        {
            // 1️⃣ Get current alive monsters
            var playerMonsters = MonsterSpawner.Instance.GetPlayerMonsters()
                .Where(m => m != null && m.IsAlive())
                .ToList();

            var enemyMonsters = MonsterSpawner.Instance.GetEnemyMonsters()
                .Where(m => m != null && m.IsAlive())
                .ToList();

            if (playerMonsters.Count == 0 || enemyMonsters.Count == 0)
            {
                Debug.Log("CombatSystem: No opponents left alive.");
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // 2️⃣ Build turn order
            List<MonsterController> turnOrder = new List<MonsterController>();
            turnOrder.AddRange(playerMonsters);
            turnOrder.AddRange(enemyMonsters);

            turnOrder = turnOrder
                .OrderByDescending(m => m.data.baseSpeed)
                .ThenBy(x => Random.value)
                .ToList();

            // 3️⃣ Process turn order
            foreach (var attacker in turnOrder)
            {
                if (!WaveManager.Instance.IsWaveRunning) yield break;

                if (!attacker.IsAlive()) continue;

                var enemyList = attacker.isPlayer ? enemyMonsters : playerMonsters;
                MonsterController target = FindValidTarget(enemyList, attacker.GetSlotIndex());

                if (target != null && target.IsAlive())
                {
                    attacker.Attack(target);
                }
                else
                {
                    Debug.Log($"{attacker.data.monsterName} had no valid target and skipped turn.");
                    BattlePanelManager.Instance.AppendCombatLog($"{attacker.data.monsterName} had no valid target and skipped turn.");
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(2.0f);
        }

        Debug.Log("CombatSystem: Wave timer ended, ending combat.");
        WaveManager.Instance.EndWave();
    }



    private MonsterController FindValidTarget(List<MonsterController> enemyList, int startIndex)
    {
        for (int i = startIndex; i < enemyList.Count; i++)
        {
            if (enemyList[i] != null && enemyList[i].IsAlive())
                return enemyList[i];
        }
        return null;
    }
}
