using UnityEngine;

[CreateAssetMenu(fileName = "NewType", menuName = "Scriptable Objects/Monster/Type")]
public class TypeSO : ScriptableObject
{
    public string typeName;
    [TextArea] public string description;
    [TextArea] public string flavorText;
    public Sprite iconSprite;
    public Color typeColor;
    public PassiveEffectData passiveEffect;
}

[System.Serializable]
public class PassiveEffectData
{
    public PassiveEffectType effectType;
    public float value1;
    public float value2;
    public StatusEffectSO statusEffect;
}