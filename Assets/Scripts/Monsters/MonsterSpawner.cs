using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public static MonsterSpawner Instance { get; private set; }

    [Header("Prefab to Spawn")]
    [SerializeField] private GameObject monsterPrefab;

    [Header("Grid Containers")]
    [SerializeField] private Transform playerSideGrid;
    [SerializeField] private Transform enemySideGrid;

    private List<GameObject> spawnedMonsters = new List<GameObject>();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ClearAllMonsters()
    {
        foreach (var monster in spawnedMonsters)
        {
            Destroy(monster);
        }
        spawnedMonsters.Clear();
    }

    public void SpawnPlayerMonsters(List<MonsterDataSO> playerPicks)
    {
        foreach (var data in playerPicks)
        {
            GameObject newMonster = Instantiate(monsterPrefab, playerSideGrid);
            MonsterController controller = newMonster.GetComponent<MonsterController>();
            controller.Setup(data, isPlayerTeam: true);

            controller.GetComponent<MonsterTypeHandler>().Initialize();

            spawnedMonsters.Add(newMonster);
        }
    }

    public void SpawnEnemyMonsters(List<MonsterDataSO> enemyPicks)
    {
        foreach (var data in enemyPicks)
        {
            GameObject newMonster = Instantiate(monsterPrefab, enemySideGrid);
            MonsterController controller = newMonster.GetComponent<MonsterController>();
            controller.Setup(data, isPlayerTeam: false);
            controller.GetComponent<MonsterTypeHandler>().Initialize();
            spawnedMonsters.Add(newMonster);
        }
    }

    public void SpawnEnemyMonster(MonsterDataSO data)
    {
        GameObject newMonster = Instantiate(monsterPrefab, enemySideGrid);
        MonsterController controller = newMonster.GetComponent<MonsterController>();
        controller.Setup(data, isPlayerTeam: false);
        controller.GetComponent<MonsterTypeHandler>().Initialize();
        spawnedMonsters.Add(newMonster);
    }

    public List<MonsterController> GetPlayerMonsters()
    {
        List<MonsterController> playerMonsters = new List<MonsterController>();
        foreach (var monsterObj in spawnedMonsters)
        {
            if (monsterObj == null) continue;
            var controller = monsterObj.GetComponent<MonsterController>();
            if (controller != null && controller.isPlayer)
            {
                playerMonsters.Add(controller);
            }
        }
        return playerMonsters;
    }

    public List<MonsterController> GetEnemyMonsters()
    {
        List<MonsterController> enemyMonsters = new List<MonsterController>();
        foreach (var monsterObj in spawnedMonsters)
        {
            if (monsterObj == null) continue;
            var controller = monsterObj.GetComponent<MonsterController>();
            if (controller != null && !controller.isPlayer)
            {
                enemyMonsters.Add(controller);
            }
        }
        return enemyMonsters;
    }


}
