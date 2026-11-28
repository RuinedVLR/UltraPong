using System.Collections;
using System.IO;
using Map;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persists the run's progress (map position + acquired upgrades) to disk as JSON,
/// so the player can quit the game and continue later.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveFileName = "savegame.json";

    public SaveData Data { get; private set; } = new SaveData();

    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void Start()
    {
        StartCoroutine(RestoreManagersWhenReady());
    }

    /// <summary>
    /// Waits for MapManager/UpgradeManager singletons to exist (they may be created in a later
    /// scene than SaveManager), then restores the saved run state into them.
    /// </summary>
    private IEnumerator RestoreManagersWhenReady()
    {
        if (!Data.hasSave)
            yield break;

        while (MapManager.Instance == null || UpgradeManager.Instance == null)
            yield return null;

        MapManager.Instance.SetProgress(Data.currentNodeIndex, Data.pendingNodeIndex, Data.pendingSceneName);
        UpgradeManager.Instance.RestoreUpgrades(Data.acquiredUpgradeNames);

        SubscribeToManagers();
    }

    private void OnEnable()
    {
        SubscribeToManagers();
    }

    private void OnDisable()
    {
        if (MapManager.Instance != null)
            MapManager.Instance.OnNodeEntered -= HandleNodeEntered;
    }

    private void SubscribeToManagers()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.OnNodeEntered -= HandleNodeEntered;
            MapManager.Instance.OnNodeEntered += HandleNodeEntered;
        }
    }

    /// <summary>
    /// The only autosave trigger: entering a new battle. Captures map progress, the battle that was
    /// just entered, and every upgrade acquired so far.
    /// </summary>
    private void HandleNodeEntered(int nodeIndex, string sceneName)
    {
        Data.currentNodeIndex = MapManager.Instance.CurrentNodeIndex;
        Data.pendingNodeIndex = nodeIndex;
        Data.pendingSceneName = sceneName;
        Save();
    }

    /// <summary>
    /// Records that an upgrade was acquired. Does NOT save immediately — it gets persisted the next
    /// time the player enters a battle (see HandleNodeEntered).
    /// </summary>
    public void RecordUpgradeAcquired(string upgradeName)
    {
        Data.acquiredUpgradeNames.Add(upgradeName);
    }

    public void Save()
    {
        Data.hasSave = true;

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Data = new SaveData();
            return;
        }

        string json = File.ReadAllText(SavePath);
        Data = JsonUtility.FromJson<SaveData>(json);
    }

    /// <summary>
    /// Wipes the save file and resets in-memory data, e.g. when starting a brand new run.
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);

        Data = new SaveData();

        if (MapManager.Instance != null)
            MapManager.Instance.ResetRun();

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.ResetUpgrades();
    }

    public bool HasSave => Data != null && Data.hasSave;

    /// <summary>
    /// Call this from the main menu's "Continue" button. If a battle was in progress when the game
    /// was last saved, loads that same battle scene directly (fresh lives/rally, as if just entered).
    /// Otherwise falls back to the map scene.
    /// </summary>
    public void ContinueGame(string mapSceneName)
    {
        if (!HasSave)
            return;

        if (!string.IsNullOrEmpty(Data.pendingSceneName))
            SceneManager.LoadScene(Data.pendingSceneName);
        else
            SceneManager.LoadScene(mapSceneName);
    }
}
