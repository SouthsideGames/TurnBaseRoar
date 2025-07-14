using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class DraftSystem : MonoBehaviour
{
    public static DraftSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int numberOfDraftChoices = 3;
    [SerializeField] private float firstPickMessageDelay = 1.5f;
    [SerializeField] private float enemyPickDelay = 1.5f;

    [Header("References")]
    [SerializeField] private MonsterLibrary monsterLibrary;
    [SerializeField] private BattlePanelManager battlePanelManager;
    [SerializeField] private MonsterManager monsterManager;

    [Header("Test:")]
    [SerializeField] private bool forceEnemyFirst = false;

    private List<MonsterDataSO> currentDraftChoices = new List<MonsterDataSO>();
    private bool playerPicked = false;
    private bool enemyPicked = false;
    private DraftTurn firstPickTurn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDraft()
    {
        battlePanelManager.HideStartDraftButton();
        battlePanelManager.HideDraftText();
        battlePanelManager.ShowDraftContentRoot();

        GenerateDraftChoices();
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

        if (forceEnemyFirst)
        {
            firstPickTurn = DraftTurn.EnemyFirst;
        }
        else
        {
            firstPickTurn = (UnityEngine.Random.value < 0.5f) ? DraftTurn.PlayerFirst : DraftTurn.EnemyFirst;
        }

        string firstPickMessage = (firstPickTurn == DraftTurn.PlayerFirst) ? "Player picks first!" : "Enemy picks first!";
        Debug.Log($"DraftSystem: {firstPickMessage}");

        StartCoroutine(ShowFirstPickAndChoices(firstPickMessage));

        playerPicked = false;
        enemyPicked = false;
    }


    private IEnumerator ShowFirstPickAndChoices(string firstPickMessage)
    {
        battlePanelManager.ShowDraftText(firstPickMessage);
        yield return new WaitForSeconds(firstPickMessageDelay);
        battlePanelManager.HideDraftText();
        battlePanelManager.ShowDraftChoices(currentDraftChoices);

        if (firstPickTurn == DraftTurn.EnemyFirst)
        {
            yield return new WaitForSeconds(enemyPickDelay);
            EnemyPick();
        }
    }




   public void PlayerPick(MonsterDataSO pickedMonster)
    {
        if (playerPicked)
        {
            Debug.LogWarning("Player has already picked!");
            return;
        }

        if (!currentDraftChoices.Contains(pickedMonster))
        {
            Debug.LogError("Invalid pick: Monster not in current choices.");
            return;
        }

        Debug.Log($"DraftSystem: Player picked {pickedMonster.monsterName}");

        playerPicked = true;

        currentDraftChoices.Remove(pickedMonster);
        monsterManager.AddPlayerDraftPick(pickedMonster);

        if (!enemyPicked)
        {
            EnemyPick();
        }

        CheckIfDraftComplete();
    }


    private void EnemyPick()
    {
        if (enemyPicked)
        {
            Debug.LogWarning("Enemy has already picked!");
            return;
        }

        if (currentDraftChoices.Count == 0)
        {
            Debug.LogError("No monsters left for enemy to pick!");
            return;
        }

        MonsterDataSO enemyPick = currentDraftChoices[UnityEngine.Random.Range(0, currentDraftChoices.Count)];

        Debug.Log($"DraftSystem: Enemy picked {enemyPick.monsterName}");

        enemyPicked = true;

        currentDraftChoices.Remove(enemyPick);
        monsterManager.AddEnemyDraftPick(enemyPick);

        // Optionally refresh UI if player picks second
        if (!playerPicked && firstPickTurn == DraftTurn.EnemyFirst)
            battlePanelManager.ShowDraftChoices(currentDraftChoices);
    }


    private void CheckIfDraftComplete()
    {
        if (playerPicked && enemyPicked)
        {
            Debug.Log("DraftSystem: Both picks complete!");

            battlePanelManager.ClearCombatLog();
            battlePanelManager.HideDraftText();

            battlePanelManager.EnableStartWaveButton();
        }
    }

}
