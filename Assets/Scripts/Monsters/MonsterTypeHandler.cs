using UnityEngine;

public class MonsterTypeHandler : MonoBehaviour
{
    public TypeSO assignedType;
    private MonsterController controller;

    private void Awake()
    {
        controller = GetComponent<MonsterController>();
    }

    public void Initialize(TypeSO type)
    {
        assignedType = type;
        ApplyPassive();
    }

    private void ApplyPassive()
    {
        if (assignedType == null || assignedType.passiveEffect == null) return;

        PassiveEffectData passive = assignedType.passiveEffect;
        Debug.Log($"[MonsterTypeHandler] Applying passive {passive.effectType} to {controller.data.monsterName}");

        controller.RegisterPassive(passive);
    }
}
