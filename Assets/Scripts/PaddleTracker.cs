using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float distanceFromCamera = 7f;
    public GameObject ball;
    [SerializeField] private EnemyProjectile enemyProjectile;
    private bool isClickable;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        enemyProjectile = GameObject.Find("Enemy").GetComponent<EnemyProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        // paddle tracks mouse position
        Vector3 mouseScreenPos = Input.mousePosition;

        mouseScreenPos.z = distanceFromCamera;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        transform.position = mouseWorldPos;

        if(isClickable == true && Input.GetMouseButton(0))
        {
            // gets new random position on the table player side. the moving to enemy bool triggers the MoveTowardsEnemy method
            enemyProjectile.selectedPoint = Random.Range(0,enemyProjectile.playerPoint.Length);
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
    private void OnCollisionEnter(Collision collision)
    {

    }
}
