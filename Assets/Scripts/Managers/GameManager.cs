using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("Scene name of the map — lives are NOT reset when returning to it.")]
    [SerializeField] private string mapSceneName = "Map";

    public bool isRalying;

    public int lives = 3;
    private int score;

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

    private void Update()
    {

    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mapSceneName)
            return;

        if (BallManager.Instance == null)
            return; // Not a battle scene (e.g. MainMenu) — nothing to reset/subscribe to.

        BallManager.Instance.OnBallScored -= LoseState;
        BallManager.Instance.OnBallScored += LoseState;

        // Entering a new level/battle scene — lives refill, but score and map progress persist.
        lives = 3;
        isRalying = true;
    }

    public void RestartGame()
    {
        lives = 3;
        score = 0;
        isRalying = true;
        UIManager.Instance.isPaused = false;
        UIManager.Instance.CloseAllUI();
        UIManager.Instance.hudCanvas.gameObject.SetActive(true);

        if (SaveManager.Instance != null)
            SaveManager.Instance.DeleteSave();
    }

    private void LoseState()
    {
        isRalying = false;
        UIManager.Instance.ShowLoseScreen();
    }
}

