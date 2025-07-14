using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject missionsPanel;
    [SerializeField] private GameObject achievementsPanel;
    [SerializeField] private GameObject inboxPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject collectionPanel;

    public void ShowMainMenuPanel() => ShowPanel(mainMenuPanel);
    public void ShowShopPanel() => ShowPanel(shopPanel);
    public void ShowLeaderboardPanel() => ShowPanel(leaderboardPanel);
    public void ShowMissionsPanel() => ShowPanel(missionsPanel);
    public void ShowAchievementsPanel() => ShowPanel(achievementsPanel);
    public void ShowInboxPanel() => ShowPanel(inboxPanel);
    public void ShowSettingsPanel() => ShowPanel(settingsPanel);
    public void ShowCollectionPanel() => ShowPanel(collectionPanel);

    public void ShowBattlePanel()
    {
        ShowPanel(battlePanel);

        // Always clear the BattlePanel UI
        if (BattlePanelManager.Instance != null)
        {
            BattlePanelManager.Instance.HideAllPanels();
        }

    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPanel(GameObject panelToShow)
    {
        HideAllPanels();
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    private void HideAllPanels()
    {
        mainMenuPanel?.SetActive(false);
        shopPanel?.SetActive(false);
        battlePanel?.SetActive(false);
        leaderboardPanel?.SetActive(false);
        missionsPanel?.SetActive(false);
        achievementsPanel?.SetActive(false);
        inboxPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        collectionPanel?.SetActive(false);
    }
    
    public void OnBattleButtonPressed()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.RestartGame();
        }

        ShowBattlePanel();
    }


}
