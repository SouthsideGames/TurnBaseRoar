using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    [SerializeField] private GameObject monsterPrefab;

    [Header("Grid Containers")]
    [SerializeField] private Transform playerSideGrid;
    [SerializeField] private Transform enemySideGrid;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    /// <summary>
    /// Clears all monsters from both grids.
    /// Call this between waves.
    /// </summary>
    public void ClearAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            Destroy(monster);
        }
        spawnedMonsters.Clear();
    }

    /// <summary>
    /// Spawns player's selected monsters in their grid.
    /// </summary>
    public void SpawnPlayerMonsters(List<MonsterDataSO> playerPicks)
    {
        foreach (var data in playerPicks)
        {
            GameObject newMonster = Instantiate(monsterPrefab, playerSideGrid);
            MonsterController controller = newMonster.GetComponent<MonsterController>();
            controller.Setup(data, isPlayerTeam: true);
            spawnedMonsters.Add(newMonster);
        }
    }

    /// <summary>
    /// Spawns enemy's selected monsters in their grid.
    /// </summary>
    public void SpawnEnemyMonsters(List<MonsterDataSO> enemyPicks)
    {
        foreach (var data in enemyPicks)
        {
            GameObject newMonster = Instantiate(monsterPrefab, enemySideGrid);
            MonsterController controller = newMonster.GetComponent<MonsterController>();
            controller.Setup(data, isPlayerTeam: false);
            spawnedMonsters.Add(newMonster);
        }
    }

    /// <summary>
    /// Spawns a single enemy monster in the enemy grid.
    /// </summary>
    public void SpawnEnemyMonster(MonsterDataSO data)
    {
        GameObject newMonster = Instantiate(monsterPrefab, enemySideGrid);
        MonsterController controller = newMonster.GetComponent<MonsterController>();
        controller.Setup(data, isPlayerTeam: false);
        spawnedMonsters.Add(newMonster);
    }

}
