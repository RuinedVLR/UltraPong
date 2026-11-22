using UnityEngine;

[CreateAssetMenu(fileName = "PaddleSizeChange", menuName = "Upgrades/Upgrade Effects/Paddle Size Change")]
public class PaddleSizeChange : UpgradeEffectData
{
    [SerializeField] private float sizeMultiplier;

    public override void Apply(UpgradeContext context)
    {
        Paddle paddle = context.Paddle;
        if (paddle != null)
        {
            paddle.CurrentSize *= sizeMultiplier;
        }
    }

    public override string GetPreviewText(UpgradeContext context)
    {
        float current = context?.Paddle != null ? context.Paddle.CurrentSize : 1f;
        float next = current * sizeMultiplier;
        return $"Paddle Size: {current:0.##} -> {next:0.##}";
    }
}
