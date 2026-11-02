using System;
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
            paddle.currentSize *= sizeMultiplier;
        }
    }
}
