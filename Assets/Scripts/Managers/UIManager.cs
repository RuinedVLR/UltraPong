using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Canvas hudCanvas;
    private TextMeshProUGUI livestext;
    private GameManager gameManager;
    private void Start()
    {
        hudCanvas = GameObject.Find("HUDcanvas").GetComponent<Canvas>();
        livestext = GameObject.Find("Livestxt").GetComponent<TextMeshProUGUI>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        livestext.text = "Lives: " + gameManager.lives.ToString();
    }
}
