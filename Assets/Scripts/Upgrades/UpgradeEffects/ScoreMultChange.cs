using UnityEngine;

[CreateAssetMenu(fileName = "ScoreMultChange", menuName = "Upgrades/Upgrade Effects/Score Mult Change")]
public class ScoreMultChange : UpgradeEffectData
{
    [SerializeField] private float scoreMultChange;

    public override void Apply(UpgradeContext context)
    {
        context.Player.scoreMultiplier += scoreMultChange;
    }

    public override string GetPreviewText(UpgradeContext context)
    {
        float current = context?.Player != null ? context.Player.scoreMultiplier : 1f;
        float next = current + scoreMultChange;
        return $"Score Multiplier: {current:0.##}x -> {next:0.##}x";
    }
}