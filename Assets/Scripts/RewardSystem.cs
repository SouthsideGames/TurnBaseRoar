using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public static RewardSystem Instance { get; private set; }

    [Header("Reward Settings")]
    [SerializeField] private int coinsPerEnemyKill = 5;
    [SerializeField] private int waveClearBonus = 50;

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

    /// <summary>
    /// Calculates coins earned this wave.
    /// </summary>
    /// <param name="enemiesKilled">How many enemies were defeated.</param>
    /// <returns>Total coins earned.</returns>
    public int CalculateWaveRewards(int enemiesKilled)
    {
        int earned = (enemiesKilled * coinsPerEnemyKill) + waveClearBonus;
        Debug.Log($"RewardSystem: Calculated {earned} coins for {enemiesKilled} kills.");
        return earned;
    }

    /// <summary>
    /// Adds coins to player's saved total.
    /// </summary>
    public void GrantRewards(int coins)
    {
        int currentCoins = SaveManager.Instance.LoadCoins();
        currentCoins += coins;
        SaveManager.Instance.SaveCoins(currentCoins);

        Debug.Log($"RewardSystem: Granted {coins} coins. New total: {currentCoins}");
    }

    /// <summary>
    /// Gets current total coins from save.
    /// </summary>
    public int GetPlayerCoins()
    {
        return SaveManager.Instance.LoadCoins();
    }
}
