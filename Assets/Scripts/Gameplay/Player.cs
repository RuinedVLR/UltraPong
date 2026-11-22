using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    // Bonus score-per-hit granted by upgrades only. Starts at 0 — the base 100 pts/hit
    public int scoreGain = 0;
    public float scoreMultiplier = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
