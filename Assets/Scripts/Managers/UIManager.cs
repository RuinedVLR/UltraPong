using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject pauseCanvas;
    public Canvas hudCanvas;
    private TextMeshProUGUI livestext;
    private GameManager gameManager;

    public bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hudCanvas = GameObject.Find("HUDcanvas").GetComponent<Canvas>();
        livestext = GameObject.Find("Livestxt").GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        livestext.text = "Lives: " + gameManager.lives.ToString();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isPaused)
            Pause();
        if (Keyboard.current.escapeKey.wasPressedThisFrame && isPaused)
            Unpause();
    }

    public void CloseAllUI()
    {
        hudCanvas.gameObject.SetActive(false);
        loseCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
    }

    public void Pause()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Unpause()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public void ShowLoseScreen()
    {
        loseCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        gameManager.RestartGame();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
