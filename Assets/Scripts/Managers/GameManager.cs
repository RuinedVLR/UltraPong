using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isRalying;

    public int lives = 3;
    private int score;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void OnEnable()
    {
        BallManager.Instance.OnBallScored += LoseState;
    }

    private void OnDisable()
    {
        BallManager.Instance.OnBallScored -= LoseState;
    }

    private void Update()
    {
        
    }


    private void LoseState()
    {
        isRalying = false;
        UIManager.Instance.ShowLoseScreen();
    }
}
