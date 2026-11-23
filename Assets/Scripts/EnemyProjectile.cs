using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class EnemyProjectile : MonoBehaviour
{
    //collection of positions that will be randomly selected depending on which state the rally is in
    public Transform[] enemyPoint;
    public Transform[] playerPoint;
    public Transform[] screenPoint;
    public Transform[] enemyScreenPoint;
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

    public float baseSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Range(0f, 100f)]
    [SerializeField]private float enemyMissChance = 90f;

    //higher number means miss chance increases slower
    [SerializeField] private float enemyMissScaling;

    private int roundsGone;

    private GameManager gameManager;

    private MatchStart matchStart;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        matchStart = GameObject.Find("GameManager").GetComponent<MatchStart>();

        speed = baseSpeed;
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
            selectedPoint = Random.Range(0, enemyScreenPoint.Length);
            targetPoint = enemyScreenPoint[selectedPoint];
  

        }
        if(Vector3.Distance(ball.transform.position, enemyScreenPoint[selectedPoint].position) <= 0.2f)
        {
            roundsGone += 1;
            enemyMissChance += roundsGone * speed / enemyMissScaling;
            if (MissChance(enemyMissChance))
            {
                movingToEnemy = false;
                movingToPlayer = false;
                movingToScreen = false;
                enemyMissChance = 0;
                roundsGone = 0;
                speed = baseSpeed;
                // add win rally multi here
                // have a missed hit keyframe for enemy keyframes will be added to a list and matched up with [selectedpoint]

                matchStart.StartCoroutine(matchStart.WaitForStart());
            }

            else
            {
                // have an enemy hit ball keyframe same logic as with above
                movingToEnemy = false;
                movingToScreen = false;
                movingToPlayer = true;
                speed += .2f;
            }

            

        }

  
    }

    public void MoveTowardsScreen()
    {
        // moves towards screen where player would either hit or miss the ball
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, screenPoint[selectedPoint].position, speed * Time.deltaTime);
        if(Vector3.Distance(ball.transform.position, screenPoint[selectedPoint].position) <= 0.2f)
        {
            gameManager.lives -= 1;
            speed = baseSpeed;
            roundsGone = 0;
            enemyMissChance = 0;
            ball.transform.position = enemyPos.position;
            movingToEnemy = false;
            movingToPlayer = false;
            movingToScreen = false;
            matchStart.StartCoroutine(matchStart.WaitForStart());
        }
    }
    public void OnRallyStart()
    {
        //placeholder method to start rally 
        selectedPoint = Random.Range(0, enemyPoint.Length);
        movingToPlayer = true;
        

    }

    public bool MissChance(float chance)
    {
        if (chance <= 0f) return false;
        if (chance >= 100f) return true;

        return Random.Range(0f, 100f) < chance;
    }
}

