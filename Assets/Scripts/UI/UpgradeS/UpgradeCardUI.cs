using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image rarityBorder;

    private UpgradeData upgrade;
    private System.Action onSelected;

    public void Setup(UpgradeData data, System.Action onSelected)
    {
        upgrade = data;
        this.onSelected = onSelected;

        icon.sprite = data.Icon;
        title.text = data.UpgradeName;
        description.text = data.Description;

        rarityBorder.color =
            UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color;
        rarityBorder.color = new Color(
            UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color.r,
            UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color.g,
            UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color.b, 
            1f);
    }

    public void Select()
    {
        UpgradeManager.Instance.AddUpgrade(upgrade);
        onSelected?.Invoke();
    }
}
