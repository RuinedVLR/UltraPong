using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public int scoreGain = 100;
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
