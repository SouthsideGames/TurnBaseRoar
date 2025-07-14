using System.Collections.Generic;
using UnityEngine;
using System;

public class DraftSystem : MonoBehaviour
{
    public static DraftSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int numberOfDraftChoices = 3;

    [Header("References")]
    [SerializeField] private MonsterLibrary monsterLibrary;
    [SerializeField] private BattlePanelManager battlePanelManager;
    [SerializeField] private MonsterManager monsterManager;

    private List<MonsterDataSO> currentDraftChoices = new List<MonsterDataSO>();
    private bool playerPicked = false;
    private bool enemyPicked = false;

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


    public void GenerateDraftChoices()
    {
        Debug.Log("DraftSystem: Generating new draft choices...");

        currentDraftChoices.Clear();

        List<MonsterDataSO> availableMonsters = new List<MonsterDataSO>(monsterLibrary.allMonsters);
        availableMonsters.Shuffle();
        for (int i = 0; i < numberOfDraftChoices && i < availableMonsters.Count; i++)
        {
            currentDraftChoices.Add(availableMonsters[i]);
        }

        battlePanelManager.ShowDraftChoices(currentDraftChoices);

        playerPicked = false;
        enemyPicked = false;
    }

    public void PlayerPick(MonsterDataSO pickedMonster)
    {
        if (playerPicked)
        {
            Debug.LogWarning("Player has already picked!");
            return;
        }

        Debug.Log($"DraftSystem: Player picked {pickedMonster.monsterName}");

        playerPicked = true;

        monsterManager.AddPlayerDraftPick(pickedMonster);

        EnemyPick();  // AI picks immediately after

        CheckIfDraftComplete();
    }

    private void EnemyPick()
    {
        if (enemyPicked)
        {
            Debug.LogWarning("Enemy has already picked!");
            return;
        }

        // AI picks randomly from the draft choices
        MonsterDataSO enemyPick = currentDraftChoices[UnityEngine.Random.Range(0, currentDraftChoices.Count)];

        Debug.Log($"DraftSystem: Enemy picked {enemyPick.monsterName}");

        enemyPicked = true;

        monsterManager.AddEnemyDraftPick(enemyPick);
    }

    private void CheckIfDraftComplete()
    {
        if (playerPicked && enemyPicked)
        {
            Debug.Log("DraftSystem: Both picks complete!");
            battlePanelManager.EnableStartWaveButton();
        }
    }
}
