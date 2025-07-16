using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    public string statusName;
    [TextArea] public string description;
    public int numberOfTurns;
    public StatusEffectType effectType;
    public float value;
    public Color statusColor = Color.white;
}
