using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Database")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeData> Upgrades;
}
