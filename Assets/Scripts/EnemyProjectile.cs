using System.Linq;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Transform[] enemyPoint;
    public Transform[] playerPoint;
    [SerializeField]private int selectedPoint;
    public bool movingToPlayer = false; 
    public Transform enemyPos;
    public GameObject ball;
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(movingToPlayer == true)
        {
            MoveTowardsPlayer();
        }
        else if (movingToPlayer == false)
        {
            MoveTowardsEnemy();
        }
    }

    public void MoveTowardsPlayer()
    {
       ball.transform.position = Vector3.MoveTowards(ball.transform.position, enemyPoint[selectedPoint].position, speed * Time.deltaTime);
    }
    public void MoveTowardsEnemy()
    {
        
    }

    public void OnRallyStart()
    {
        selectedPoint = Random.Range(0, enemyPoint.Length);
        movingToPlayer = true;
        

    }
}
