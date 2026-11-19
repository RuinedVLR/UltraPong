using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Tooltip("Scene name of the map — these references are not re-acquired there")]
    [SerializeField] private string mapSceneName = "Map";

    private Player player;
    private BallManager ballManager;
    private Paddle paddle;
    private UpgradePanel upgradePanel;

    [SerializeField] private UpgradeDatabase database;

    [SerializeField] private List<RaritySettings> raritySettings;

    private readonly List<string> acquiredUpgradeNames = new();

    private UpgradeContext context;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mapSceneName)
            return;

        if (!RebindSceneReferences())
            return; // Not a battle scene (e.g. MainMenu) — nothing to do here.

        ReapplyAcquiredUpgrades();
    }

    private bool RebindSceneReferences()
    {
        player = FindFirstObjectByType<Player>();
        ballManager = FindFirstObjectByType<BallManager>();
        paddle = FindFirstObjectByType<Paddle>();
        upgradePanel = FindFirstObjectByType<UpgradePanel>(FindObjectsInactive.Include);

        if (player == null && ballManager == null && paddle == null && upgradePanel == null)
            return false; // None found — this isn't a battle scene at all.

        context = new UpgradeContext
        {
            Player = player,
            BallManager = ballManager,
            Paddle = paddle
        };

        if (player == null || ballManager == null || paddle == null || upgradePanel == null)
            Debug.LogWarning("UpgradeManager could not find all required scene references (Player/BallManager/Paddle/UpgradePanel) in the loaded battle scene.");

        return true;
    }

    public void ChooseUpgrades()
    {
        if (upgradePanel == null)
            return;

        upgradePanel.Show();
    }

    public RaritySettings GetRaritySettings(UpgradeRarity rarity)
    {
        foreach (var settings in raritySettings)
        {
            if (settings.Rarity == rarity)
            {
                return settings;
            }
        }

        return null;
    }

    public void AddUpgrade(UpgradeData upgrade)
    {
        foreach (var effect in upgrade.Effects)
        {
            effect.Apply(context);
        }

        if (!acquiredUpgradeNames.Contains(upgrade.name))
            acquiredUpgradeNames.Add(upgrade.name);

        if (SaveManager.Instance != null)
            SaveManager.Instance.RecordUpgradeAcquired(upgrade.name);
    }

    /// <summary>
    /// Re-applies every upgrade acquired so far this run to the current scene's Player/Paddle.
    /// Needed because those gameplay objects are recreated fresh in every battle scene.
    /// </summary>
    private void ReapplyAcquiredUpgrades()
    {
        if (database == null)
            return;

        foreach (var upgradeName in acquiredUpgradeNames)
        {
            UpgradeData upgrade = database.Upgrades.Find(u => u.name == upgradeName);
            if (upgrade == null)
            {
                Debug.LogWarning($"UpgradeManager: could not find acquired upgrade \"{upgradeName}\" in the database.");
                continue;
            }

            foreach (var effect in upgrade.Effects)
            {
                effect.Apply(context);
            }
        }
    }

    /// <summary>
    /// Adds a previously saved list of upgrades (by UpgradeData asset name) to the acquired list, e.g. on game load.
    /// Does not apply effects directly — ReapplyAcquiredUpgrades (called on every battle scene load) handles that.
    /// </summary>
    public void RestoreUpgrades(List<string> upgradeNames)
    {
        if (upgradeNames == null)
            return;

        foreach (var upgradeName in upgradeNames)
        {
            if (!acquiredUpgradeNames.Contains(upgradeName))
                acquiredUpgradeNames.Add(upgradeName);
        }
    }
}

