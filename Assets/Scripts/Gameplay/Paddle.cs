using UnityEngine;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    public float currentSize = 1f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
