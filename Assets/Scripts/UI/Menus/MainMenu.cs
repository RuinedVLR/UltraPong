using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the first scene of the game (e.g., the map scene)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test_Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
