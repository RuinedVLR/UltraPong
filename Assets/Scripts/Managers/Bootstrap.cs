using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lives only in the Bootstrap scene. Runs once at game startup, after all persistent
/// managers (SaveManager, GameManager, MapManager, UpgradeManager, ...) have initialized
/// in their own Awake(), then hands off to the Main Menu (or straight to the map).
/// </summary>
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
