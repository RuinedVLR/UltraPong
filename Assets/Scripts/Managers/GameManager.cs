using System.Collections;
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

        // Entering a new level/battle scene — lives refill, but score and map progress persist.
        lives = 3;
        isRalying = true;

        // sceneLoaded fires BEFORE Start() runs on the new scene's objects, so BallManager.Instance
        // isn't set yet here. Wait a frame (by which point Start() has run) before subscribing.
        StartCoroutine(SubscribeToBallManagerNextFrame());
    }

    private IEnumerator SubscribeToBallManagerNextFrame()
    {
        yield return null;

        if (BallManager.Instance == null)
            yield break; // Not a battle scene

        BallManager.Instance.OnBallScored -= LoseState;
        BallManager.Instance.OnBallScored += LoseState;
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

        SceneManager.LoadScene(mapSceneName);
    }

    private void LoseState()
    {
        isRalying = false;
        SaveManager.Instance.DeleteSave();
        UIManager.Instance.ShowLoseScreen();
    }
}

