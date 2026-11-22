using UnityEngine;

public abstract class UpgradeEffectData : ScriptableObject
{
    public abstract void Apply(UpgradeContext context);

    /// <summary>
    /// Returns a short "current -> new" preview string for the upgrade card UI,
    /// </summary>
    public abstract string GetPreviewText(UpgradeContext context);
}
