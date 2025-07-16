using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;


public class MonsterController : MonoBehaviour
{
    [Header("Runtime Info")]
    public MonsterDataSO data;
    public bool isPlayer;

    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image typeImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider healthSlider;

    [Header("References")]
    public PassiveEffectData currentPassive;
    [SerializeField] private int slotIndex;
    private bool isImmuneToStatus = false;
    private bool isImmuneToCrit = false;
    
    private StatusEffectSO activeStatusEffect;
    private int statusTurnsRemaining;
    private int currentHP;
    private int currentMana;
    private Color defaultNameColor;

  
    public void Setup(MonsterDataSO monsterData, bool isPlayerTeam)
    {
        data = monsterData;
        isPlayer = isPlayerTeam;
        currentHP = data.baseHP;
        currentMana = 0;
        Debug.Log($"MonsterController: {data.monsterName} spawned. Team: {(isPlayer ? "Player" : "Enemy")} with {data.baseHP} HP and {data.baseAttack} Attack.");

        // Set icon and name in UI
        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (typeImage != null && data.monsterType.iconSprite != null)
            typeImage.sprite = data.monsterType.iconSprite;

        if (nameText != null)
        {
            nameText.text = data.monsterName;
            defaultNameColor = nameText.color;
        }


        // Set up health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = data.baseHP;
            healthSlider.value = currentHP;
        }

        
    }


    public void TakeDamage(int amount, bool isCrit = false)
    {
        currentHP -= amount;

        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} takes {amount} damage{(isCrit ? " (CRIT!)" : "")}. Remaining HP: {currentHP}");

        UpdateHealthBar();

        if (currentHP <= 0)
        {
            OnDeath();
        }
    }



    public void Attack(MonsterController target)
    {
        if (target == null || !target.IsAlive()) return;

        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s turn!");

        // 1️⃣ Mana regeneration
        currentMana = Mathf.Min(currentMana + data.manaRegenPerTurn, data.maxMana);
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} regenerates mana to {currentMana}/{data.maxMana}.");

        // 2️⃣ Decide if using ability
        bool canUseAbility = currentMana >= data.abilityManaCost;
        bool wantsToUseAbility = Random.value < 0.5f;

        if (canUseAbility && wantsToUseAbility)
        {
            UseAbility(target);
            currentMana -= data.abilityManaCost;
            return;
        }

        // 3️⃣ Else, do regular attack
        PerformBasicAttack(target);
    }

    private void PerformBasicAttack(MonsterController target)
    {
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} attacks {target.data.monsterName}!");

        // Simple accuracy check (no evasion)
        if (Random.value >= data.baseAccuracy)
        {
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s attack missed!");
            return;
        }

        // Crit check
        bool isCrit = CheckCrit(data.critChance, target.data.critResist);

        // Damage calculation
        float damage = CalculateDamage(
            data.baseAttack,
            target.data.baseDefense,
            isCrit,
            data.critMultiplier
        );

        target.TakeDamage(Mathf.RoundToInt(damage), isCrit);

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.BurnOnHit)
        {
            float chance = currentPassive.value1;
            if (Random.value < (chance / 100f))
            {
                target.ApplyStatusEffect(currentPassive.statusEffect);
                Debug.Log($"{data.monsterName} inflicted {currentPassive.statusEffect.statusName} on {target.data.monsterName}!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} inflicted {currentPassive.statusEffect.statusName} on {target.data.monsterName}!");
            }
        }
    }



    private void UseAbility(MonsterController target)
    {
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} uses special ability: {data.abilityDescription}");

        // Placeholder effect: 1.5x attack ignoring crit
        float abilityDamage = data.baseAttack * 1.5f;

        float finalDamage = abilityDamage * Mathf.Clamp01(1f - (target.data.baseDefense / 100f));
        target.TakeDamage(Mathf.RoundToInt(finalDamage), isCrit: false);
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

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHP;
        }
    }

    public bool IsAlive() => currentHP > 0;

    private bool CheckCrit(int attackerCritChance, int defenderCritResist)
    {
        int effectiveCritChance = Mathf.Max(0, attackerCritChance - defenderCritResist);
        return Random.Range(0, 100) < effectiveCritChance;
    }

    private float CalculateDamage(float attackPower, float defensePercent, bool isCrit, int critMultiplier)
    {
        float defenseFactor = Mathf.Clamp01(1f - (defensePercent / 100f));
        float baseDamage = attackPower * defenseFactor;

        if (isCrit && !isImmuneToCrit)
            baseDamage *= critMultiplier;

        return baseDamage;
    }

    public void RegisterPassive(PassiveEffectData passive)
    {
        currentPassive = passive;

        if (passive == null) return;

        Debug.Log($"{data.monsterName} registered passive: {passive.effectType}");

        // Example one-time setup
        switch (passive.effectType)
        {
            case PassiveEffectType.ImmuneToStatus:
                // Set flag on this monster
                isImmuneToStatus = true;
                break;

            case PassiveEffectType.ImmuneToCrit:
                isImmuneToCrit = true;
                break;

            // Add more setup for other types as needed
        }
    }

    private void Heal(float amount)
    {
        currentHP += Mathf.CeilToInt(amount);
        if (currentHP > data.baseHP)
            currentHP = data.baseHP;

        UpdateHealthBar();
    }

#region PASSIVE ABILITIES 

    public void ApplyEndOfTurnPassive(List<MonsterController> playerMonsters, List<MonsterController> enemyMonsters)
    {
        if (currentPassive == null) return;
    
        if (currentPassive.effectType == PassiveEffectType.HealSelfPerTurn)
        {
            int healAmount = Mathf.CeilToInt(data.baseHP * (currentPassive.value1 / 100f));
            Heal(healAmount);
            Debug.Log($"{data.monsterName} healed {healAmount} HP at end of turn (HealSelfPerTurn).");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} healed {healAmount} HP!");
        }
        else if (currentPassive.effectType == PassiveEffectType.TeamHealPerTurn)
        {
            List<MonsterController> allies = isPlayer ? playerMonsters : enemyMonsters;
    
            foreach (var ally in allies)
            {
                if (ally == null || !ally.IsAlive()) continue;
    
                int healAmount = Mathf.CeilToInt(ally.data.baseHP * (currentPassive.value1 / 100f));
                ally.Heal(healAmount);
    
                Debug.Log($"{data.monsterName} healed {ally.data.monsterName} for {healAmount} HP (TeamHealPerTurn).");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} heals {ally.data.monsterName} for {healAmount} HP!");
            }
        }

    }
#endregion

#region STATUS EFFECTS
    
    public void ApplyStatusEffect(StatusEffectSO newStatus)
    {
        if (isImmuneToStatus)
        {
            Debug.Log($"{data.monsterName} is immune to status effects!");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} is immune to status effects!");
            return;
        }

        if (activeStatusEffect != null)
        {
            Debug.Log($"{data.monsterName} already has {activeStatusEffect.statusName} and cannot be inflicted with {newStatus.statusName}.");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} resisted {newStatus.statusName} because it's already afflicted!");
            return;
        }

        activeStatusEffect = newStatus;
        statusTurnsRemaining = newStatus.duration;

        // ✅ NEW: Change name text color to status color
        if (nameText != null)
            nameText.color = newStatus.statusColor;

        Debug.Log($"{data.monsterName} is now afflicted with {newStatus.statusName} for {statusTurnsRemaining} turns.");
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} is now afflicted with {newStatus.statusName} for {statusTurnsRemaining} turns!");
    }



    public void ProcessStatusEffect()
    {
        if (activeStatusEffect == null) return;

        // Apply effect logic
        switch (activeStatusEffect.effectType)
        {
            case StatusEffectType.DamageOverTurn:
                int dotDamage = Mathf.CeilToInt(activeStatusEffect.value);
                TakeDamage(dotDamage);
                Debug.Log($"{data.monsterName} took {dotDamage} damage from {activeStatusEffect.statusName}!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} takes {dotDamage} damage from {activeStatusEffect.statusName}!");
                break;

            case StatusEffectType.Freeze:
                // Example: no damage, but skip turn. We'll hook this into Attack() later.
                break;

            // Add other effect types here.
        }

        // Decrement duration
        statusTurnsRemaining--;

        if (statusTurnsRemaining <= 0)
        {
            ClearStatusEffect();
        }
    }

    public void ClearStatusEffect()
    {
        if (activeStatusEffect != null)
        {
            Debug.Log($"{data.monsterName} is no longer affected by {activeStatusEffect.statusName}!");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} is no longer affected by {activeStatusEffect.statusName}!");
        }

        activeStatusEffect = null;
        statusTurnsRemaining = 0;

       
        if (nameText != null)
            nameText.color = defaultNameColor;
    }



#endregion

    public void SetSlotIndex(int index) => slotIndex = index;
    public int GetSlotIndex() => slotIndex;


}
