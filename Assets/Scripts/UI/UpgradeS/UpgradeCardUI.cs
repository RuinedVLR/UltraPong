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

    public void Setup(UpgradeData data)
    {
        upgrade = data;

        icon.sprite = data.Icon;
        title.text = data.UpgradeName;
        description.text = data.Description;

        rarityBorder.color =
            UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color;
    }

    public void Select()
    {
        UpgradeManager.Instance.AddUpgrade(upgrade);
    }
}
