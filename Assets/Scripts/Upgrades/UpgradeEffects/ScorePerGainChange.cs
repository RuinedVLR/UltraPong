using UnityEngine;

[CreateAssetMenu(fileName = "ScorePerGainChange", menuName = "Upgrades/Upgrade Effects/Score Per Gain Change")]
public class ScorePerGainChange : UpgradeEffectData
{
    [SerializeField] private int scoreGainChange;

    public override void Apply(UpgradeContext context)
    {
        context.Player.scoreGain += scoreGainChange;
    }

    public override string GetPreviewText(UpgradeContext context)
    {
        int currentBonus = context?.Player != null ? context.Player.scoreGain : 0;
        int nextBonus = currentBonus + scoreGainChange;

        const int baseScoreGain = 100; // keep in sync with PaddleTracker.baseScoreGain
        return $"Score per Hit: {baseScoreGain + currentBonus} -> {baseScoreGain + nextBonus}";
    }
}
