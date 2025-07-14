using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIManager uiManager;

    private GameState currentState;

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
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.MainMenu:
                uiManager.ShowMainMenuPanel();
                break;
            case GameState.Shop:
                uiManager.ShowShopPanel();
                break;
            case GameState.Battle:
                uiManager.ShowBattlePanel();
                break;
            case GameState.Leaderboard:
                uiManager.ShowLeaderboardPanel();
                break;
            case GameState.Missions:
                uiManager.ShowMissionsPanel();
                break;
            case GameState.Achievements:
                uiManager.ShowAchievementsPanel();
                break;
            case GameState.Inbox:
                uiManager.ShowInboxPanel();
                break;
            case GameState.Settings:
                uiManager.ShowSettingsPanel();
                break;
            case GameState.Collection:
                uiManager.ShowCollectionPanel();
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        SetState(GameState.MainMenu);
    }

}
