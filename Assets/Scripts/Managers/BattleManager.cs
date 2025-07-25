using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private BattlePanelManager battlePanelManager;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private DraftSystem draftSystem;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private RewardSystem rewardSystem;
    [SerializeField] private Button continueButton;

    [Header("Settings")]
    [SerializeField] private int startingWave = 1;
    [SerializeField] private int playerCoins = 0;

    private BattlePhase currentPhase;
    private int currentWave;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        playerCoins = SaveManager.Instance.LoadCoins();
        currentWave = startingWave;
    }

    public void EnterDraftPhase()
    {
        currentPhase = BattlePhase.Draft;

        Debug.Log("Entering Draft Phase");
        battlePanelManager.ShowDraftPhaseUI();
        battlePanelManager.UpdateWaveNumber(currentWave);
        battlePanelManager.UpdateCoins(playerCoins);
    }


    public void StartCombatPhase()
    {
        currentPhase = BattlePhase.Combat;

        Debug.Log("Starting Combat Phase");
        battlePanelManager.ShowCombatPhaseUI();

        waveManager.StartWave();
        combatSystem.StartCombat();
    }

    public void EnterResultsPhase(int coinsEarned)
    {
        currentPhase = BattlePhase.Results;

        Debug.Log($"Entering Results Phase. Coins Earned: {coinsEarned}");

        playerCoins += coinsEarned;
        SaveManager.Instance.SaveCoins(playerCoins);

        battlePanelManager.UpdateCoins(playerCoins);
        battlePanelManager.ShowResultsPhaseUI(coinsEarned);
    }

    public void ConfirmResultsAndContinue()
    {
        currentWave++;
        UIManager.Instance.ShowBattlePanel();
    }


    // Optional method to spend coins
    public bool TrySpendCoins(int cost)
    {
        if (playerCoins >= cost)
        {
            playerCoins -= cost;
            SaveManager.Instance.SaveCoins(playerCoins);
            battlePanelManager.UpdateCoins(playerCoins);
            return true;
        }

        return false;
    }

    public int GetPlayerCoins()
    {
        return playerCoins;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public void EnterGameOver(int totalCoins)
    {
        currentPhase = BattlePhase.Results;
        Debug.Log($"Game Over! Coins Earned: {totalCoins}");
        MonsterSpawner.Instance.ClearAllMonsters();

        // Show the game over screen with player's final team
        battlePanelManager.ShowGameOverUI(totalCoins, new List<OwnedMonster>(MonsterManager.Instance.TeamMonsters));
    }
    
    public void RestartGame()
    {
        Debug.Log("Restarting game from wave 1.");
        currentWave = startingWave;
        MonsterManager.Instance.ClearAllData();
        RewardSystem.Instance.ResetRunRewards();
        EnterDraftPhase();
    }


    
}
