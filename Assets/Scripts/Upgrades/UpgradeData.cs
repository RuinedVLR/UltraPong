using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string UpgradeName;

    [TextArea]
    public string Description;

    public Sprite Icon;

    public UpgradeRarity Rarity;

    [Range(1, 100)]
    public int Weight = 100;

    public UpgradeEffectData[] Effects;
}
