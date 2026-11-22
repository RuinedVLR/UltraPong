using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float distanceFromCamera = 0.8f;
    public GameObject ball;
    [SerializeField] private BallManager enemyProjectile;
    [SerializeField] private int baseScoreGain = 100;
    private bool isClickable;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        enemyProjectile = FindFirstObjectByType<BallManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // paddle tracks mouse position
        Vector3 mouseScreenPos = Input.mousePosition;

        mouseScreenPos.z = distanceFromCamera;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        transform.position = mouseWorldPos;

        if (isClickable == true && enemyProjectile.movingToScreen == true && Input.GetMouseButtonDown(0))
        {
            enemyProjectile.selectedPoint = Random.Range(0, enemyProjectile.playerPoint.Length);
            isClickable = false;
            enemyProjectile.targetPoint = enemyProjectile.playerPoint[enemyProjectile.selectedPoint];
            enemyProjectile.movingToEnemy = true;
            enemyProjectile.movingToPlayer = false;
            enemyProjectile.movingToScreen = false;

            int gained = Mathf.RoundToInt((baseScoreGain + Player.Instance.scoreGain) * Player.Instance.scoreMultiplier);
            GameManager.Instance.AddScore(gained);
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("within range");
        // checks if ball can be hit
        isClickable = true;

    }
    private void OnTriggerExit(Collider other)
    {
        // prevents a stale click from the previous rally firing during the next countdown
        isClickable = false;
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
}
