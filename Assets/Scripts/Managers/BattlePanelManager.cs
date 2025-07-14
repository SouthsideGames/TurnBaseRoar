using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;


public class BattlePanelManager : MonoBehaviour
{
     public static BattlePanelManager Instance { get; private set; }

    [Header("Panels / Sections")]
    [SerializeField] private GameObject draftBar;
    [SerializeField] private GameObject rewardSummary;
    [SerializeField] private GameObject combatBar;
    
    [Header("Buttons")]
    [SerializeField] private Button startWaveButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button continueButton;


    [Header("Texts")]
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text rewardSummaryText;
    [SerializeField] private TMP_Text combatLogText;


    [Header("Grids")]
    [SerializeField] private Transform playerSideGrid;
    [SerializeField] private Transform enemySideGrid;

    [Header("Draft Bar")]
    [SerializeField] private Transform draftContentRoot;
    [SerializeField] private GameObject draftCardPrefab;

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

    private void Start()
    {
        // Optionally, hook up buttons to methods
        startWaveButton.onClick.AddListener(OnStartWavePressed);
        finishButton.onClick.AddListener(OnFinishPressed);
        returnButton.onClick.AddListener(OnReturnPressed);
        continueButton.onClick.AddListener(OnContinuePressed);
    }

    public void ShowDraftPhaseUI()
    {
        draftBar.SetActive(true);
        startWaveButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        rewardSummary.SetActive(false);
        combatBar.SetActive(false);

        ClearCombatLog();

    }

    public void EnableStartWaveButton()
    {
        startWaveButton.gameObject.SetActive(true);
    }

    public void ShowCombatPhaseUI()
    {
        draftBar.SetActive(false);
        startWaveButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        rewardSummary.SetActive(false);
        combatBar.SetActive(true);
    }

    public void ShowResultsPhaseUI(int coinsEarned)
    {
        draftBar.SetActive(false);
        startWaveButton.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(true);
        combatBar.SetActive(false);
        rewardSummary.SetActive(true);
        continueButton.gameObject.SetActive(true);


        rewardSummaryText.text = $"You earned {coinsEarned} coins!";
    }

    public void UpdateWaveNumber(int wave)
    {
        waveNumberText.text = $"Wave: {wave}";
    }

    public void UpdateCoins(int coins)
    {
        coinsText.text = $"Coins: {coins}";
    }

    public void UpdateTimer(float timeLeft)
    {
        timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";
    }

    // Example button callbacks
    private void OnStartWavePressed()
    {
        BattleManager.Instance.StartCombatPhase();
    }

    private void OnFinishPressed()
    {
        BattleManager.Instance.ConfirmResultsAndContinue();
    }

    private void OnReturnPressed()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void ShowDraftChoices(List<MonsterDataSO> choices)
    {
        // Clear previous cards
        foreach (Transform child in draftContentRoot)
        {
            Destroy(child.gameObject);
        }

        // Spawn new cards
        foreach (var monster in choices)
        {
            GameObject card = Instantiate(draftCardPrefab, draftContentRoot);
            MonsterDraftCard cardScript = card.GetComponent<MonsterDraftCard>();
            cardScript.Setup(monster, this);
        }
    }

    public void OnDraftCardSelected(MonsterDataSO picked)
    {
        Debug.Log($"BattlePanelManager: Player picked {picked.monsterName}");
        DraftSystem.Instance.PlayerPick(picked);
    }

    public void HideResultsPhaseUI()
    {
        rewardSummary.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    private void OnContinuePressed()
    {
        BattleManager.Instance.ConfirmResultsAndContinue();
        HideResultsPhaseUI();
    }

    public void AppendCombatLog(string message)
    {
        if (combatLogText != null)
        {
            combatLogText.text += "\n" + message;
        }
    }

    public void ClearCombatLog()
    {
        if (combatLogText != null)
        {
            combatLogText.text = "";
        }
    }



}
