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

        // The battle the player had entered but not yet finished when the game was saved/quit.
        // -1 / empty means no battle is pending (player was on the map).
        public int pendingNodeIndex = -1;
        public string pendingSceneName = "";

        // Upgrades acquired so far in this run (by UpgradeData.name, the asset name)
        public List<string> acquiredUpgradeNames = new List<string>();

        public bool hasSave;
    }
}
