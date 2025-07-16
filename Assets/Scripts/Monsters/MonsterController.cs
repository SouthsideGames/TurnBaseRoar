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

    private float currentDefense;
    private int currentMana;
    private Color defaultNameColor;
    private bool hasAttackedThisWave;
    private bool isUntargetable = false;
    private int untargetableTurnsRemaining = 0;
    private float damageBonusPercent = 0f;
    private float currentStatusResistance;



    public void Setup(MonsterDataSO monsterData, bool isPlayerTeam)
    {
        data = monsterData;
        isPlayer = isPlayerTeam;
        currentHP = data.baseHP;
        currentDefense = data.baseDefense;
        currentStatusResistance = data.baseStatusResist;
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

        hasAttackedThisWave = false;
        isUntargetable = false;
        untargetableTurnsRemaining = 0;

    }


    public void TakeDamage(int amount, bool isCrit = false)
    {
        currentHP -= amount;

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.UntargetableOnHit)
        {
            isUntargetable = true;
            untargetableTurnsRemaining = Mathf.CeilToInt(currentPassive.value1);

            Debug.Log($"{data.monsterName} became untargetable for {untargetableTurnsRemaining} turn(s)!");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} became untargetable!");
        }


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

        if (Random.value >= data.baseAccuracy)
        {
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s attack missed!");
            return;
        }

        bool isCrit = CheckCrit(data.critChance, target.data.critResist);

        float damage = CalculateDamage(
            data.baseAttack,
            target.currentDefense,
            isCrit,
            data.critMultiplier
        );

        // ✅ BonusFirstAttack passive
        if (!hasAttackedThisWave && currentPassive != null && currentPassive.effectType == PassiveEffectType.BonusFirstAttack)
        {
            float bonusPercent = currentPassive.value1;
            damage += damage * (bonusPercent / 100f);
            hasAttackedThisWave = true;

            Debug.Log($"{data.monsterName} gains {bonusPercent}% bonus damage on first attack!");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} deals bonus damage on first attack!");
        }


        target.TakeDamage(Mathf.RoundToInt(damage), isCrit);

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.Lifesteal)
        {
            float lifestealPercent = currentPassive.value1;
            int healAmount = Mathf.CeilToInt(damage * (lifestealPercent / 100f));

            if (healAmount > 0)
            {
                Heal(healAmount);
                Debug.Log($"{data.monsterName} lifestealed {healAmount} HP!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} lifestealed {healAmount} HP!");
            }
        }


        if (currentPassive != null &&
            (currentPassive.effectType == PassiveEffectType.BurnOnHit || currentPassive.effectType == PassiveEffectType.FreezeChance))
        {
            float chance = currentPassive.value1;
            float modifiedChance = chance - target.currentStatusResistance;
            if (modifiedChance < 0) modifiedChance = 0;

            if (Random.value < (modifiedChance / 100f))
            {
                target.ApplyStatusEffect(currentPassive.statusEffect);
                Debug.Log($"{data.monsterName} inflicted {currentPassive.statusEffect.statusName} on {target.data.monsterName}!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} inflicted {currentPassive.statusEffect.statusName} on {target.data.monsterName}!");
            }
        }

        // ✅ Check for MultiHit passive
        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.MultiHit)
        {
            float reductionPercent = currentPassive.value1; // e.g. 25 means 25%

            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} strikes again!");

            // Crit check for second hit
            bool isCrit2 = CheckCrit(data.critChance, target.data.critResist);

            // Damage calculation for second hit
            float damage2 = CalculateDamage(
                data.baseAttack,
                target.currentDefense,
                isCrit2,
                data.critMultiplier
            );

            damage2 = damage2 * (reductionPercent / 100f);

            target.TakeDamage(Mathf.RoundToInt(damage2), isCrit2);

            Debug.Log($"{data.monsterName} dealt {damage2} damage on second hit (MultiHit).");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} hits again for {Mathf.RoundToInt(damage2)} damage!");
        }

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.ReduceDefense)
        {
            target.ReduceDefense(currentPassive.value1);
        }

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.ReduceStatusResistance)
        {
            target.ReduceStatusResistance(currentPassive.value1);
        }

        if (currentPassive != null && currentPassive.effectType == PassiveEffectType.WyrmCleave)
        {
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} uses Wyrm Cleave to hit all enemies!");

            List<MonsterController> enemyTeam = isPlayer ? CombatSystem.Instance.GetEnemyMonsters() : CombatSystem.Instance.GetPlayerMonsters();

            foreach (var enemy in enemyTeam)
            {
                if (enemy == null || !enemy.IsAlive()) continue;

                float cleaveDamage = CalculateDamage(
                    data.baseAttack,
                    enemy.currentDefense,
                    isCrit,
                    data.critMultiplier
                );

                enemy.TakeDamage(Mathf.RoundToInt(cleaveDamage), isCrit);
            }

            return;
        }

        
    }

    private void UseAbility(MonsterController target)
    {
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} uses special ability: {data.abilityDescription}");

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

    #region HELPER FUNCTIONS

    public bool IsAlive() => currentHP > 0;

    private bool CheckCrit(int attackerCritChance, int defenderCritResist)
    {
        int effectiveCritChance = Mathf.Max(0, attackerCritChance - defenderCritResist);
        return Random.Range(0, 100) < effectiveCritChance;
    }

    private float CalculateDamage(float attackPower, float defensePercent, bool isCrit, int critMultiplier)
    {
        float defenseFactor = Mathf.Clamp01(1f - (defensePercent / 100f));
        float modifiedAttack = attackPower * (1 + damageBonusPercent / 100f);
        float baseDamage = modifiedAttack * defenseFactor;

        if (isCrit && !isImmuneToCrit)
            baseDamage *= critMultiplier;

        return baseDamage;
    }

    public void ReduceDefense(float amount)
    {
        currentDefense -= amount;
        if (currentDefense < 0) currentDefense = 0;

        Debug.Log($"{data.monsterName}'s defense reduced by {amount}. Now at {currentDefense}%.");
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s defense was reduced by {amount}%!");
    }

    public void ReduceStatusResistance(float amount)
    {
        currentStatusResistance -= amount;
        if (currentStatusResistance < 0) currentStatusResistance = 0;

        Debug.Log($"{data.monsterName}'s status resistance reduced by {amount}%. Now at {currentStatusResistance}%.");
        BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s resistance was reduced by {amount}%!");
    }


    public bool IsTargetable()
    {
        return IsAlive() && !isUntargetable;
    }

    public void ProcessUntargetable()
    {
        if (isUntargetable)
        {
            untargetableTurnsRemaining--;
            if (untargetableTurnsRemaining <= 0)
            {
                isUntargetable = false;
                Debug.Log($"{data.monsterName} is now targetable again!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} is now targetable again!");
            }
        }
    }


    private void Heal(float amount)
    {
        currentHP += Mathf.CeilToInt(amount);
        if (currentHP > data.baseHP)
            currentHP = data.baseHP;

        UpdateHealthBar();
    }
    
#endregion

    #region PASSIVE ABILITIES 

    public void RegisterPassive(PassiveEffectData passive)
    {
        currentPassive = passive;

        if (passive == null) return;

        Debug.Log($"{data.monsterName} registered passive: {passive.effectType}");

        switch (passive.effectType)
        {
            case PassiveEffectType.ImmuneToStatus:
                isImmuneToStatus = true;
                break;

            case PassiveEffectType.ImmuneToCrit:
                isImmuneToCrit = true;
                break;

        }
    }

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
        else if (currentPassive.effectType == PassiveEffectType.DamageRamp)
        {
            damageBonusPercent += currentPassive.value1;

            Debug.Log($"{data.monsterName}'s damage ramped up by {currentPassive.value1}%. Total bonus = {damageBonusPercent}%.");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName}'s damage increased by {currentPassive.value1}%!");
        }
        else if (currentPassive.effectType == PassiveEffectType.WyrmCleave)
        {
            int selfDamage = Mathf.CeilToInt(data.baseHP * (currentPassive.value1 / 100f));
            TakeDamage(selfDamage);

            Debug.Log($"{data.monsterName} takes {selfDamage} self-damage from Wyrm Cleave passive.");
            BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} takes {selfDamage} damage from Wyrm Cleave!");
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
        statusTurnsRemaining = newStatus.numberOfTurns;

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
                Debug.Log($"{data.monsterName} is frozen and cannot act!");
                BattlePanelManager.Instance.AppendCombatLog($"{data.monsterName} is frozen and cannot act this turn!");
                break;

                // Add other effect types here.
        }

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
