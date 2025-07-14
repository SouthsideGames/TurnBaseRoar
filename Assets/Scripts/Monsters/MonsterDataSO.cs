using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Scriptable Objects/Monster/MonsterData")]
public class MonsterDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string monsterID;
    public string monsterName;
    public MonsterType type;
    public Sprite icon;

    [Header("Base Stats")]
    public int baseHP;
    public int baseAttack;
    [Range(0, 100)]
    public float baseDefense;         // As percentage: 0–100. Used as % reduction.
    public int baseSpeed;
    public string attackName;

    [Header("Combat Stats")]
    [Range(0f, 1f)]
    public float baseAccuracy = 0.9f; // Chance to hit
    [Range(0, 100)]
    public int critChance = 10;       // % chance to crit
    public int critMultiplier = 2;    // Damage multiplier on crit
    [Range(0, 100)]
    public int critResist = 0;        // Reduces enemy crit chance

    [Header("Mana Stats")]
    public int maxMana = 100;         // Max mana for ability use
    public int manaRegenPerTurn = 10; // Regen per turn
    public int abilityManaCost = 50;  // Cost to use special ability

    [Header("Ability")]
    [TextArea]
    public string abilityDescription; // Placeholder text
    //public AbilityDataSO abilityData; // Future implementation hook

    [Header("Evolution")]
    public int evolutionLevel;
    public MonsterDataSO evolutionForm;
}
