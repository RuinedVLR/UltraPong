using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Scene names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string mapSceneName = "Map";

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private Button continueButton;

    [Header("Battle / HUD")]
    public Canvas hudCanvas;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI targetScoreText;
    [SerializeField] private TextMeshProUGUI rallyScoreText;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private UpgradePanel upgradePanel;

    [Header("Run Results (after defeating a boss)")]
    [SerializeField] private GameObject runResultsCanvas;
    //[SerializeField] private TextMeshProUGUI runResultsScoreText;
    //[SerializeField] private TextMeshProUGUI runResultsLevelsText;

    [Header("Tutorial")]
    [SerializeField] private TutorialPanel tutorialPanel;

    private GameManager gameManager;

    private Coroutine scoreAnimCoroutine;
    private Coroutine rallyScoreAnimCoroutine;

    public bool isPaused;

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

    private void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void Update()
    {
        if (gameManager != null && livesText != null)
            livesText.text = "Lives: " + gameManager.lives;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused && hudCanvas != null && hudCanvas.gameObject.activeSelf)
                Pause();
            else if (isPaused)
                Unpause();
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMainMenu = scene.name == mainMenuSceneName;
        bool isMap = scene.name == mapSceneName;
        bool isBattle = !isMainMenu && !isMap;

        CloseAllUI();

        if (isMainMenu)
        {
            ShowMainMenuPanel();

            if (continueButton != null)
                continueButton.interactable = SaveManager.Instance != null && SaveManager.Instance.HasSave;
        }

        if (isBattle)
        {
            gameManager = GameManager.Instance;

            if (hudCanvas != null)
                hudCanvas.gameObject.SetActive(true);
        }
    }

    public void CloseAllUI()
    {
        if (hudCanvas != null) hudCanvas.gameObject.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (upgradePanel != null) upgradePanel.gameObject.SetActive(false);
        if (runResultsCanvas != null) runResultsCanvas.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.Hide();
    }

    #region Score HUD
    public void SetScoreTextImmediate(int value)
    {
        if (scoreAnimCoroutine != null)
            StopCoroutine(scoreAnimCoroutine);

        if (scoreText != null)
            scoreText.text = "Current Score: " + value;
    }

    public void SetPendingScoreTextImmediate(int value)
    {
        if (rallyScoreAnimCoroutine != null)
            StopCoroutine(rallyScoreAnimCoroutine);

        if (rallyScoreText != null)
            rallyScoreText.text = value.ToString();
    }

    public void SetTargetScoreText(int target)
    {
        if (targetScoreText != null)
            targetScoreText.text = "Score To Beat: " + target;
    }

    public void AnimateScore(int from, int to, float duration)
    {
        if (scoreAnimCoroutine != null)
            StopCoroutine(scoreAnimCoroutine);

        scoreAnimCoroutine = StartCoroutine(AnimateTextRoutine(scoreText, "Current Score: ", from, to, duration));
    }

    public void AnimatePendingScore(int from, int to, float duration)
    {
        if (rallyScoreAnimCoroutine != null)
            StopCoroutine(rallyScoreAnimCoroutine);

        if (duration <= 0f)
        {
            if (rallyScoreText != null)
                rallyScoreText.text = "Rally: " + to;
            return;
        }

        rallyScoreAnimCoroutine = StartCoroutine(AnimateTextRoutine(rallyScoreText, "Rally: ", from, to, duration));
    }

    private IEnumerator AnimateTextRoutine(TextMeshProUGUI target, string prefix, int from, int to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));

            if (target != null)
                target.text = prefix + current;

            yield return null;
        }

        if (target != null)
            target.text = prefix + to;
    }
    #endregion

    #region Pause
    public void Pause()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
    #endregion

    #region Lose / Win / Results
    public void ShowLoseScreen()
    {
        loseCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowWinScreen()
    {
        winCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideWinScreen()
    {
        winCanvas.SetActive(false);
    }

    public void ShowUpgradeSelection()
    {
        upgradePanel.Show();
    }

    public void ShowRunResultsScreen(int finalScore, int levelsCleared)
    {
        if (runResultsCanvas == null)
            return;

        runResultsCanvas.SetActive(true);

        //if (runResultsScoreText != null)
        //    runResultsScoreText.text = $"Score: {finalScore}";

        //if (runResultsLevelsText != null)
        //    runResultsLevelsText.text = $"Levels cleared: {levelsCleared}";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region Main Menu
    public void ShowMainMenuPanel()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void NewGame()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();

        if (tutorialPanel != null)
        {
            mainMenuCanvas.SetActive(false);
            tutorialPanel.Show(StartNewGameAfterTutorial);
        }
        else
        {
            StartNewGameAfterTutorial();
        }
    }

    private void StartNewGameAfterTutorial()
    {
        SceneManager.LoadScene(mapSceneName);
        SaveManager.Instance.Save();
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
            SaveManager.Instance.ContinueGame(mapSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
        Time.timeScale = 1f; // Reset time scale in case it was paused
    }
}
