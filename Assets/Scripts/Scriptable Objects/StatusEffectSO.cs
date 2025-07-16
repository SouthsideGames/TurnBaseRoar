using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Scriptable Objects/Monster/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    public string statusName;
    [TextArea] public string description;
    public Sprite iconSprite;
    public int duration;
    public StatusEffectType effectType;
    public float value;
    public Color statusColor = Color.white;
}
