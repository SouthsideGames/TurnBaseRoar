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
    [SerializeField] private GameObject gameoverPanel;


    [Header("Buttons")]
    [SerializeField] private Button startWaveButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button startDraftButton;



    [Header("Texts")]
    [SerializeField] private TMP_Text waveNumberText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text rewardSummaryText;
    [SerializeField] private TMP_Text combatLogText;
    [SerializeField] private TMP_Text totalCoinsText;
    [SerializeField] private TMP_Text draftText;


    [Header("Grids")]
    [SerializeField] private Transform playerSideGrid;
    [SerializeField] private Transform enemySideGrid;
    [SerializeField] private Transform playerTeamContainer;

    [Header("Draft Bar")]
    [SerializeField] private Transform draftContentRoot;
    [SerializeField] private GameObject draftCardPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {

        startWaveButton.onClick.AddListener(OnStartWavePressed);
        finishButton.onClick.AddListener(OnFinishPressed);
        returnButton.onClick.AddListener(OnReturnPressed);
        continueButton.onClick.AddListener(OnContinuePressed);
        mainMenuButton.onClick.AddListener(OnMainMenuPressed);
        restartButton.onClick.AddListener(OnRestartPressed);
        startDraftButton.onClick.AddListener(OnStartDraftPressed);
    }

    private void OnMainMenuPressed()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    private void OnRestartPressed()
    {
        BattleManager.Instance.RestartGame();
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

        HideDraftContentRoot(); 
        ShowStartDraftButton(); 
        ShowDraftText("Press Start Draft to see who goes first!"); 
    }

    private void OnStartDraftPressed()
    {
        DraftSystem.Instance.StartDraft();
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

        foreach (Transform child in draftContentRoot)
            Destroy(child.gameObject);

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
            combatLogText.text += "\n" + message;
    }

    public void ClearCombatLog()
    {
        if (combatLogText != null)
            combatLogText.text = "";
    }

    public void ShowDraftText(string message)
    {
        if (draftText != null)
        {
            draftText.text = message;
            draftText.gameObject.SetActive(true);
        }
    }

    public void HideDraftText()
    {
        if (draftText != null)
            draftText.gameObject.SetActive(false);
    }

    public void ShowStartDraftButton()
    {
        if (startDraftButton != null)
            startDraftButton.gameObject.SetActive(true);
    }

    public void HideStartDraftButton()
    {
        if (startDraftButton != null)
            startDraftButton.gameObject.SetActive(false);
    }

    public void ShowDraftContentRoot()
    {
        if (draftContentRoot != null)
            draftContentRoot.gameObject.SetActive(true);
    }

    public void HideDraftContentRoot()
    {
        if (draftContentRoot != null)
            draftContentRoot.gameObject.SetActive(false);
    }


    public void ShowGameOverUI(int totalCoins, List<OwnedMonster> playerTeam)
    {
        HideAllPanels();
        gameoverPanel.SetActive(true);

        totalCoinsText.text = $"Total Coins Won: {totalCoins}";

        foreach (Transform child in playerTeamContainer)
            Destroy(child.gameObject);

        // Add team monsters
        foreach (var monster in playerTeam)
        {
            var textObj = new GameObject("MonsterName", typeof(RectTransform), typeof(TMP_Text));
            textObj.transform.SetParent(playerTeamContainer, false);
            var tmp = textObj.GetComponent<TMP_Text>();
            tmp.text = $"{monster.data.monsterName} (Level {monster.level})";
            tmp.fontSize = 32;
        }
    }

   public void HideAllPanels()
    {
        
        if (rewardSummary != null) rewardSummary.SetActive(false);
        if (gameoverPanel != null) gameoverPanel.SetActive(false);
        if (combatBar != null) combatBar.SetActive(false);

        if (startWaveButton != null) startWaveButton.gameObject.SetActive(false);
        if (finishButton != null) finishButton.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

}
