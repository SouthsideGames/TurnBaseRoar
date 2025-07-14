using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterLibrary", menuName = "Scriptable Objects/Monster/MonsterLibrary")]
public class MonsterLibrary : ScriptableObject
{
    public List<MonsterDataSO> allMonsters;

    private Dictionary<string, MonsterDataSO> idLookup;

    public void Initialize()
    {
        idLookup = new Dictionary<string, MonsterDataSO>();
        foreach (var monster in allMonsters)
        {
            if (!idLookup.ContainsKey(monster.monsterID))
                idLookup.Add(monster.monsterID, monster);
        }
    }

    public MonsterDataSO GetMonsterByID(string id)
    {
        if (idLookup == null)
            Initialize();

        if (idLookup.TryGetValue(id, out var result))
            return result;

        Debug.LogError($"Monster ID not found: {id}");
        return null;
    }

    public MonsterDataSO GetRandomMonster()
    {
        if (allMonsters == null || allMonsters.Count == 0)
        {
            Debug.LogError("MonsterLibrary: No monsters available!");
            return null;
        }

        int index = Random.Range(0, allMonsters.Count);
        return allMonsters[index];
    }

}
