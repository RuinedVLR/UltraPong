using System.Collections;
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
    private bool isOnCooldown;
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

        if (isClickable == false && Input.GetMouseButtonDown(0) && isOnCooldown == false)
        {
            //starts cooldown period if swung with no ball in range
            StartCoroutine(ClickCooldown());
        }
        else if (isClickable == false && Input.GetMouseButtonDown(0) && isOnCooldown == true)
        {
            Debug.Log("On cooldown!");
            // have a message display in game or visual to indicate paddle swings on cooldown
        }
        else if (isClickable == true && enemyProjectile.movingToScreen == true && Input.GetMouseButtonDown(0) && isOnCooldown == false)
        {
            // gets new random position on the table player side. the moving to enemy bool triggers the MoveTowardsEnemy method
            enemyProjectile.selectedPoint = Random.Range(0, enemyProjectile.playerPoint.Length);
            Debug.Log("clicked");
            isClickable = false;
            enemyProjectile.targetPoint = enemyProjectile.playerPoint[enemyProjectile.selectedPoint];
            enemyProjectile.movingToEnemy = true;
            enemyProjectile.movingToPlayer = false;
            enemyProjectile.movingToScreen = false;

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

    private IEnumerator ClickCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(2);
        isOnCooldown = false;
    }
}
