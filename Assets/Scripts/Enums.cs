public enum MonsterType
{
    Fire,
    Water,
    Grass,
    Electric,
    Normal
}

public enum GameState
{
    MainMenu,
    Shop,
    Battle,
    Leaderboard,
    Missions,
    Achievements,
    Inbox,
    Settings,
    Collection
}

public enum BattlePhase
{
    Draft,
    Combat,
    Results
}

public enum DraftTurn
{
    PlayerFirst,
    EnemyFirst
}

public enum PassiveEffectType
{
    HealSelfPerTurn,
    TeamHealPerTurn,
    BurnOnHit,
    MultiHit,
    FreezeChance,
    ReduceDefense,
    Lifesteal,
    ImmuneToCrit,
    ImmuneToStatus,
    BonusFirstAttack,
    UntargetableOnHit,
    DamageRamp,
    ReduceStatusResistance,
    AoEHitWithRecoil,
    ReverseSpeedOrder,
    None
}

public enum StatusEffectType
{
    DamageOverTurn,
    Freeze,
    Burn,
    HealOverTurn,
    DefenseDown,
    Other
}


