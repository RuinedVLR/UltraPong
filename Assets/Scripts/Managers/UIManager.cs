using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject pauseCanvas;
    private Canvas hudCanvas;
    private TextMeshProUGUI livestext;
    private GameManager gameManager;

    public bool isPaused = false;

    private void Start()
    {
        hudCanvas = GameObject.Find("HUDcanvas").GetComponent<Canvas>();
        livestext = GameObject.Find("Livestxt").GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        CloseAllUI();
        hudCanvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        livestext.text = "Lives: " + gameManager.lives.ToString();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isPaused)
            Pause();
        if (Keyboard.current.escapeKey.wasPressedThisFrame && isPaused)
            Unpause();
    }

    void CloseAllUI()
    {
        hudCanvas.gameObject.SetActive(false);
        loseCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
    }

    void Pause()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
    }

    void Unpause()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
    }

    public void ShowLoseScreen()
    {
        loseCanvas.SetActive(true);
    }
}
