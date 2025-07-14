using System;
using UnityEngine;

[Serializable]
public class OwnedMonster
{
    public MonsterDataSO data;
    public int level;
    public int currentXP;

    public OwnedMonster(MonsterDataSO monsterData)
    {
        data = monsterData;
        level = 1;
        currentXP = 0;
    }
}
