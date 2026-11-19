using System;
using System.Collections.Generic;

namespace SaveSystem
{
    /// <summary>
    /// Plain data container that gets serialized to disk.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // Map progress
        public int currentNodeIndex = -1;

        // Upgrades acquired so far in this run (by UpgradeData.name, the asset name)
        public List<string> acquiredUpgradeNames = new List<string>();

        public bool hasSave;
    }
}
