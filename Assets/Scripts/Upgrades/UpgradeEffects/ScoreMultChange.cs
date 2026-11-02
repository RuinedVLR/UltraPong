using UnityEngine;

[CreateAssetMenu(fileName = "ScoreMultChange", menuName = "Upgrades/Upgrade Effects/Score Mult Change")]
public class ScoreMultChange : UpgradeEffectData
{
    [SerializeField] private float scoreMultChange;

    public override void Apply(UpgradeContext context)
    {
        context.Player.scoreMultiplier += scoreMultChange;
    }
}