using UnityEngine;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    [SerializeField] private float currentSize = 1f;
    private Vector3 baseScale;

    public float CurrentSize
    {
        get => currentSize;
        set
        {
            currentSize = value;
            ApplyScale();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        baseScale = transform.localScale;
        ApplyScale();
    }

    private void ApplyScale()
    {
        transform.localScale = baseScale * currentSize;
    }
}
