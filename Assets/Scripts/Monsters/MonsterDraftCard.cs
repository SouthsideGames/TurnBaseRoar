using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterDraftCard : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    private MonsterDataSO data;
    private BattlePanelManager battlePanelManager;

    /// <summary>
    /// Sets up this card with data and manager reference.
    /// </summary>
    public void Setup(MonsterDataSO monster, BattlePanelManager manager)
    {
        data = monster;
        battlePanelManager = manager;

        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
        }

        if (nameText != null)
        {
            nameText.text = data.monsterName;
        }
    }

    /// <summary>
    /// Called by UI Button when the player clicks this card.
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"MonsterDraftCard: Player selected {data.monsterName}");
        battlePanelManager.OnDraftCardSelected(data);
    }
}
