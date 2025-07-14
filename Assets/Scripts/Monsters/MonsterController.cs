using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MonsterController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackInterval = 1.5f;

    [Header("Runtime Info")]
    public MonsterDataSO data;
    public bool isPlayer;

    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider healthSlider;


    private int currentHP;
    private float attackCooldown;

    private void Start()
    {
        attackCooldown = attackInterval;
    }

    private void Update()
    {
        if (data == null) return;

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f)
        {
            MonsterController target = FindNearestTarget();
            if (target != null)
            {
                TryAttack(target);
                attackCooldown = attackInterval;
            }
        }
    }


    public void Setup(MonsterDataSO monsterData, bool isPlayerTeam)
    {
        data = monsterData;
        isPlayer = isPlayerTeam;
        currentHP = data.baseHP;

        Debug.Log($"MonsterController: {data.monsterName} spawned. Team: {(isPlayer ? "Player" : "Enemy")} with {data.baseHP} HP and {data.baseAttack} Attack.");

        // Set icon and name in UI
        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.monsterName;

        // Set up health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = data.baseHP;
            healthSlider.value = currentHP;
        }
    }


    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{data.monsterName} took {amount} damage. HP left: {currentHP}");

        UpdateHealthBar();

        if (currentHP <= 0)
        {
            OnDeath();
        }
    }



    private void TryAttack(MonsterController target)
    {
        // Roll for hit success
        if (Random.value > data.successRate)
        {
            Debug.Log($"{data.monsterName} missed!");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} tried to attack {target.data.monsterName} but missed!");
            return;
        }

        // Determine damage
        int damage = data.baseAttack;
        bool isCrit = Random.Range(0, 100) < data.critChance;
        if (isCrit)
        {
            damage *= data.critMultiplier;
        }

        // Apply damage
        target.TakeDamage(damage);

        // Log
        string critText = isCrit ? " (Critical!)" : "";
        Debug.Log($"{data.monsterName} hit {target.data.monsterName} for {damage} damage{critText}.");
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} attacked {target.data.monsterName} for {damage} damage{critText}.");
    }



    private void OnDeath()
    {
        Debug.Log($"{data.monsterName} has died.");

        if (!isPlayer)
        {
            WaveManager.Instance.RegisterEnemyKill();
        }

        Destroy(gameObject);
    }

    private MonsterController FindNearestTarget()
    {
        // Very simple targeting: find any enemy in scene
        MonsterController[] allMonsters = FindObjectsOfType<MonsterController>();
        MonsterController closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var other in allMonsters)
        {
            if (other == this) continue;
            if (other.isPlayer == this.isPlayer) continue; // skip same team

            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < closestDist)
            {
                closest = other;
                closestDist = dist;
            }
        }

        return closest;
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }
    }


}
