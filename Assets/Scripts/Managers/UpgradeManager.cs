using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private Player player;
    private BallManager ballManager;
    private Paddle paddle;
    private UpgradePanel upgradePanel;

    [SerializeField] private UpgradeDatabase database;

    [SerializeField] private List<RaritySettings> raritySettings;

    private readonly List<string> acquiredUpgradeNames = new();

    private UpgradeContext context;

    public UpgradeContext Context => context;

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
        if (!RebindSceneReferences())
            return; // Not a battle scene (MainMenu, Map, ...) — nothing to do here.

        ReapplyAcquiredUpgrades();
    }

    /// <summary>
    /// Looks for Player/BallManager/Paddle in the just-loaded scene. Returns false (silently)
    /// if none of them exist — that means this isn't a battle scene at all (MainMenu, Map, etc.),
    /// which is expected and not an error.
    /// If some but not all are found, logs a warning — that indicates a misconfigured battle scene.
    /// </summary>
    private bool RebindSceneReferences()
    {
        player = FindFirstObjectByType<Player>();
        ballManager = FindFirstObjectByType<BallManager>();
        paddle = FindFirstObjectByType<Paddle>();
        upgradePanel = FindFirstObjectByType<UpgradePanel>(FindObjectsInactive.Include);

        bool anyFound = player != null || ballManager != null || paddle != null;
        if (!anyFound)
            return false;

        context = new UpgradeContext
        {
            Player = player,
            BallManager = ballManager,
            Paddle = paddle
        };

        bool allFound = player != null && ballManager != null && paddle != null;
        if (!allFound)
            Debug.LogWarning("UpgradeManager found some but not all required scene references (Player/BallManager/Paddle) in the loaded scene — check that this battle scene is set up correctly.");

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

        // Allow duplicates: each entry represents one actual pick, so ReapplyAcquiredUpgrades
        // can correctly re-stack the upgrade the same number of times across scenes.
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

        acquiredUpgradeNames.AddRange(upgradeNames);
    }

    /// <summary>
    /// Clears every acquired upgrade, e.g. when starting a brand new run.
    /// </summary>
    public void ResetUpgrades()
    {
        acquiredUpgradeNames.Clear();
    }
}

