using UnityEngine;

public class MonsterTypeHandler : MonoBehaviour
{
    private TypeSO assignedType;
    private MonsterController controller;

    private void Awake() => controller = GetComponent<MonsterController>();
    
    public void Initialize()
    {
        if (controller == null || controller.data == null || controller.data.monsterType == null)
        {
            Debug.LogError("[MonsterTypeHandler] Missing MonsterDataSO or TypeSO!");
            return;
        }

        assignedType = controller.data.monsterType;

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
