using System;
using System.Collections.Generic;

[Serializable]
public class OwnedMonsterSaveDataList
{
    public List<OwnedMonsterSaveData> monsterSaveData;

    public OwnedMonsterSaveDataList(List<OwnedMonsterSaveData> list)
    {
        monsterSaveData = list;
    }
}
