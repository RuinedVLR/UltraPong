using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeSelector : MonoBehaviour
{
    [SerializeField]
    private UpgradeDatabase database;

    public List<UpgradeData> GetRandomUpgrades(int count = 3)
    {
        return database.Upgrades
            .OrderBy(x => Random.value)
            .Take(count)
            .ToList();
    }
}
