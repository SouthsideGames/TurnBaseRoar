using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    [Header("Reward Settings")]
    [SerializeField] private int coinsPerEnemyKill = 5;
    [SerializeField] private int waveClearBonus = 50;

    public int TotalCoinsEarned { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public int CalculateWaveRewards(int enemiesKilled)
    {
        int earned = (enemiesKilled * coinsPerEnemyKill) + waveClearBonus;
        Debug.Log($"RewardSystem: Calculated {earned} coins for {enemiesKilled} kills.");
        return earned;
    }

    public void GrantRewards(int coins)
    {
        TotalCoinsEarned += coins;  // ✅ Track total earned in this run

        int currentCoins = SaveManager.Instance.LoadCoins();
        currentCoins += coins;
        SaveManager.Instance.SaveCoins(currentCoins);

        Debug.Log($"RewardSystem: Granted {coins} coins. New total: {currentCoins}");
    }


    public int GetPlayerCoins()
    {
        return SaveManager.Instance.LoadCoins();
    }
    
    public void ResetRunRewards()
    {
        TotalCoinsEarned = 0;
    }
}
