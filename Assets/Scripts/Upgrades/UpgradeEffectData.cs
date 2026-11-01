using UnityEngine;

public abstract class UpgradeEffectData : ScriptableObject
{
    public abstract void Apply(UpgradeContext context);
}
