using System.Linq;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Transform[] enemyPoint;
    public Transform[] playerPoint;
    public Transform[] screenPoint;
    public Transform targetPoint;
    public int selectedPoint;
    public bool movingToPlayer = false;
    public bool movingToScreen = false;
    public bool movingToEnemy = false;
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
        if (movingToPlayer == true)
        {
            MoveTowardsPlayer();
        }
        if (movingToEnemy == true)
        {
            MoveTowardsEnemy();
        }
        if (movingToScreen == true)
        {
            MoveTowardsScreen();
        }
    }

    public void MoveTowardsPlayer()
    {
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, enemyPoint[selectedPoint].position, speed * Time.deltaTime);
        //simulates bouncing off table towards player
        if(Vector3.Distance(ball.transform.position, enemyPoint[selectedPoint].position) <= 0.2f)
        {

            selectedPoint = Random.Range(0, screenPoint.Length);
            movingToPlayer = false;
            movingToScreen = true;
            
           
        }

    }
    public void MoveTowardsEnemy()
    {
        
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, targetPoint.position, speed * Time.deltaTime);
        if(Vector3.Distance(ball.transform.position, playerPoint[selectedPoint].position) <= 0.2f)
        {
            targetPoint = enemyPos;
        }
        if(Vector3.Distance(ball.transform.position, enemyPos.position) <= 0.2f)
        {
            movingToEnemy = false;
            movingToScreen = false;
            movingToPlayer = true;
        }

  
    }

    public void MoveTowardsScreen()
    {
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, screenPoint[selectedPoint].position, speed * Time.deltaTime);
        if(Vector3.Distance(ball.transform.position, screenPoint[selectedPoint].position) <= 0.2f)
        {
            //fail state here
        }
    }
    public void OnRallyStart()
    {
        selectedPoint = Random.Range(0, enemyPoint.Length);
        movingToPlayer = true;
        

    }
}
