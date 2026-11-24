using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text valueChangeText;
    [SerializeField] private Image rarityBorder;

    private UpgradeData upgrade;
    private Action onSelected;

    public void Setup(UpgradeData data, Action onSelected)
    {
        upgrade = data;
        this.onSelected = onSelected;

        icon.sprite = data.Icon;
        title.text = data.UpgradeName;
        description.text = data.Description;

        var rarityColor = UpgradeManager.Instance.GetRaritySettings(data.Rarity).Color;
        rarityColor.a = 1f;
        rarityBorder.color = rarityColor;

        if (valueChangeText != null)
            valueChangeText.text = BuildValueChangeText(data);
    }

    private string BuildValueChangeText(UpgradeData data)
    {
        var context = UpgradeManager.Instance.Context;
        var lines = new List<string>();

        foreach (var effect in data.Effects)
        {
            string preview = effect.GetPreviewText(context);
            if (!string.IsNullOrEmpty(preview))
                lines.Add(preview);
        }

        return string.Join("\n", lines);
    }

    public void Select()
    {
        UpgradeManager.Instance.AddUpgrade(upgrade);
        onSelected?.Invoke();
    }
}
