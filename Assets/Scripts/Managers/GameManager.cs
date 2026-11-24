using System.Collections;
using Map;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("Scene name of the map")]
    [SerializeField] private string mapSceneName = "Map";

    [Header("Score targets per run level (hardcoded for now)")]
    [SerializeField] private int[] levelScoreTargets = { 500, 1000, 1500 };

    public bool isRalying;

    public int lives = 3;
    public int score;
    public int pendingScore;
    public int currentLevelIndex;

    private bool isBossBattle;

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
        if (BallManager.Instance != null)
            BallManager.Instance.OnBallScored -= LoseState;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mapSceneName)
            return;

        lives = 3;
        score = 0;
        pendingScore = 0;
        isRalying = true;

        var sceneInfo = FindFirstObjectByType<BattleSceneInfo>();
        isBossBattle = sceneInfo != null && sceneInfo.isBossBattle;

        UIManager.Instance.SetScoreTextImmediate(score);
        UIManager.Instance.SetPendingScoreTextImmediate(pendingScore);
        UIManager.Instance.SetTargetScoreText(GetCurrentScoreTarget());

        StartCoroutine(SubscribeToBallManagerNextFrame());
    }

    private IEnumerator SubscribeToBallManagerNextFrame()
    {
        yield return null;

        if (BallManager.Instance == null)
            yield break;

        BallManager.Instance.OnBallScored -= LoseState;
        BallManager.Instance.OnBallScored += LoseState;
    }

    public void RestartGame()
    {
        lives = 3;
        score = 0;
        pendingScore = 0;
        currentLevelIndex = 0;
        isRalying = true;
        UIManager.Instance.isPaused = false;
        UIManager.Instance.CloseAllUI();

        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        SceneManager.LoadScene(mapSceneName);
    }

    /// <summary>
    /// Called on every successful hit over the net. Accumulates into the pending (not-yet-committed)
    /// bucket for the current rally and updates the HUD immediately.
    /// </summary>
    public void AddScore(int amount)
    {
        if (!isRalying)
            return;

        pendingScore += amount;
        UIManager.Instance.AnimatePendingScore(pendingScore - amount, pendingScore, 0.25f);
    }

    /// <summary>
    /// Called by BallManager when a life is lost: commits the pending score into the total
    /// and animates both HUD counters.
    /// </summary>
    public void CommitPendingScore()
    {
        int oldScore = score;
        int oldPending = pendingScore;

        score += pendingScore;
        pendingScore = 0;

        UIManager.Instance.AnimateScore(oldScore, score, 1f);
        UIManager.Instance.AnimatePendingScore(oldPending, 0, 0.3f);
    }

    public bool HasReachedScoreTarget()
    {
        return score >= GetCurrentScoreTarget();
    }

    public int GetCurrentScoreTarget()
    {
        int index = Mathf.Clamp(currentLevelIndex, 0, levelScoreTargets.Length - 1);
        return levelScoreTargets[index];
    }

    public void CompleteLevel()
    {
        isRalying = false;

        if (MapManager.Instance != null)
            MapManager.Instance.CompletePendingNode();

        if (isBossBattle)
        {
            // Boss defeated: show final run results and wipe the save — the run is over.
            if (SaveManager.Instance != null)
                SaveManager.Instance.DeleteSave();

            UIManager.Instance.ShowRunResultsScreen(score, currentLevelIndex + 1);
        }
        else
        {
            UIManager.Instance.ShowWinScreen();
        }
    }

    // Called by the normal Win panel's "Continue" button
    public void OnWinContinue()
    {
        UIManager.Instance.HideWinScreen();
        UIManager.Instance.ShowUpgradeSelection();
    }

    // Called once the player picks an upgrade
    public void OnUpgradeChosenContinue()
    {
        currentLevelIndex++;
        SceneManager.LoadScene(mapSceneName);
    }

    // Called by the Run Results panel's button after defeating a boss
    public void OnRunResultsContinue()
    {
        currentLevelIndex = 0;
        score = 0;
        UIManager.Instance.BackToMenu();
    }

    private void LoseState()
    {
        isRalying = false;
        SaveManager.Instance.DeleteSave();
        UIManager.Instance.ShowLoseScreen();
    }
}

