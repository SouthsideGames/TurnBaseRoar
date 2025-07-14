[System.Serializable]
public class OwnedMonsterSaveData
{
    public string monsterID;
    public int level;
    public int currentXP;

    public OwnedMonsterSaveData(OwnedMonster owned)
    {
        monsterID = owned.data.monsterID;
        level = owned.level;
        currentXP = owned.currentXP;
    }
}
