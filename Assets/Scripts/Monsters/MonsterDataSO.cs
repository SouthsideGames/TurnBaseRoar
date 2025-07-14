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
    public int baseDefense;
    public int baseSpeed;
    public string attackName;

    [Header("Combat Stats")]
    public float successRate = 0.9f;  // 90% hit chance by default
    public int critChance = 10;       // 10% chance to crit
    public int critMultiplier = 2;    // double damage


    [Header("Evolution")]
    public int evolutionLevel;
    public MonsterDataSO evolutionForm;
}
