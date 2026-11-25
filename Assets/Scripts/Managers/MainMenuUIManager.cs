using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private Button continueButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowMainMenuPanel();

        if(SaveManager.Instance != null && SaveManager.Instance.HasSave)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

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

        UnityEngine.SceneManagement.SceneManager.LoadScene("Map");

        SaveManager.Instance.Save();
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
            SaveManager.Instance.ContinueGame("Map");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
