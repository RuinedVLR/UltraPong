using UnityEngine;

[CreateAssetMenu(fileName = "ScorePerGainChange", menuName = "Upgrades/Upgrade Effects/Score Per Gain Change")]
public class ScorePerGainChange : UpgradeEffectData
{
    [SerializeField] private int scoreGainChange;

    public override void Apply(UpgradeContext context)
    {
        context.Player.scoreGain += scoreGainChange;
    }
}
