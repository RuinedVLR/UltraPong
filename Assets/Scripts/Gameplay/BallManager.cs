using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }

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
    public float speed = 1f;

    public float baseSpeed;

    [SerializeField] private int enemyMissChance = 0;
    [SerializeField] private int missedNum;

    private int roundsGone;

    private GameManager gameManager;

    public event Action OnBallScored;

    private MatchStart matchStart;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        gameManager = GameManager.Instance;
        matchStart = FindFirstObjectByType<MatchStart>();

        if (matchStart == null)
            Debug.LogError("BallManager could not find a MatchStart in the scene.");

        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isRalying)
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
    }

    public void MoveTowardsPlayer()
    {
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, enemyPoint[selectedPoint].position, speed * Time.deltaTime);
        //simulates bouncing off table towards player
        if (Vector3.Distance(ball.transform.position, enemyPoint[selectedPoint].position) <= 0.2f)
        {
            selectedPoint = UnityEngine.Random.Range(0, screenPoint.Length);
            movingToPlayer = false;
            movingToScreen = true;
        }

    }
    public void MoveTowardsEnemy()
    {
        // moves ball towards point on players side of the table before returning to the enemies position
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, targetPoint.position, speed * Time.deltaTime);
        if (Vector3.Distance(ball.transform.position, playerPoint[selectedPoint].position) <= 0.2f)
        {
            targetPoint = enemyPos;
        }
        if (Vector3.Distance(ball.transform.position, enemyPos.position) <= 0.2f)
        {
            roundsGone += 1;
            enemyMissChance = roundsGone * ((int)speed) / 2;
            if (enemyMissChance >= 100)
            {
                enemyMissChance = 100;
            }
            missedNum = UnityEngine.Random.Range(enemyMissChance, 101);

            if (missedNum == enemyMissChance)
            {
                // rally win score multi here, start new rally
                return;
            }
            else
                movingToEnemy = false;
            movingToScreen = false;
            movingToPlayer = true;
            speed += .2f;
        }
    }

    public void MoveTowardsScreen()
    {
        // moves towards screen where player would either hit or miss the ball
        ball.transform.position = Vector3.MoveTowards(ball.transform.position, screenPoint[selectedPoint].position, speed * Time.deltaTime);
        if (Vector3.Distance(ball.transform.position, screenPoint[selectedPoint].position) <= 0.2f)
        {
            gameManager.lives -= 1;
            speed = baseSpeed;
            ball.transform.position = enemyPos.position;
            movingToEnemy = false;
            movingToPlayer = false;
            movingToScreen = false;

            StartCoroutine(EndOfRallyRoutine());
        }
    }

    private IEnumerator EndOfRallyRoutine()
    {
        gameManager.CommitPendingScore();

        yield return new WaitForSeconds(2f);

        if (gameManager.HasReachedScoreTarget())
        {
            gameManager.CompleteLevel();
        }
        else if (gameManager.lives <= 0)
        {
            OnBallScored?.Invoke();
        }
        else
        {
            if (matchStart != null)
                matchStart.StartCoroutine(matchStart.WaitForStart());
            else
                Debug.LogError("BallManager cannot restart the rally: matchStart reference is missing.");
        }
    }

    public void OnRallyStart()
    {
        //placeholder method to start rally 
        selectedPoint = UnityEngine.Random.Range(0, enemyPoint.Length);
        movingToPlayer = true;
    }
}
