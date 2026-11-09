using System.Linq;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    //collection of positions that will be randomly selected depending on which state the rally is in
    public Transform[] enemyPoint;
    public Transform[] playerPoint;
    public Transform[] screenPoint;
    public Transform targetPoint;

    // the currently randomly selected number that correlates to a position in the list
    public int selectedPoint;

    // state bools could probably use a state manager
    public bool movingToPlayer = false;
    public bool movingToScreen = false;
    public bool movingToEnemy = false;

    
    public Transform enemyPos;
    public GameObject ball;

    //speed float could incrimentally increase as rally continues
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]private int enemyMissChance = 0;
    [SerializeField]private int missedNum;

    private int roundsGone;
    
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
        // moves ball towards point on players side of the table before returning to the enemies position
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, targetPoint.position, speed * Time.deltaTime);
        if(Vector3.Distance(ball.transform.position, playerPoint[selectedPoint].position) <= 0.2f)
        {
            targetPoint = enemyPos;
        }
        if(Vector3.Distance(ball.transform.position, enemyPos.position) <= 0.2f)
        {
            roundsGone += 1;
            enemyMissChance = roundsGone + ((int)speed) / 2;
            if(enemyMissChance >= 100)
            {
                enemyMissChance = 100;
            }
            missedNum = Random.Range(enemyMissChance, 101);

            if(missedNum == enemyMissChance)
            {
                // rally win score multi here, start new rally
                return;
            }
            else
            movingToEnemy = false;
            movingToScreen = false;
            movingToPlayer = true;
            speed += 1.2f;
            
        }

  
    }

    public void MoveTowardsScreen()
    {
        // moves towards screen where player would either hit or miss the ball
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, screenPoint[selectedPoint].position, speed * Time.deltaTime);
        if(Vector3.Distance(ball.transform.position, screenPoint[selectedPoint].position) <= 0.2f)
        {
            //fail state here
        }
    }
    public void OnRallyStart()
    {
        //placeholder method to start rally 
        selectedPoint = Random.Range(0, enemyPoint.Length);
        movingToPlayer = true;
        

    }
}
