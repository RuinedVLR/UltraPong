using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchStart : MonoBehaviour
{
    private EnemyProjectile enemyProjectile;

    private void Start()
    {
        if (gameObject != null) DontDestroyOnLoad(gameObject);
        enemyProjectile = GameObject.Find("Enemy").GetComponent<EnemyProjectile>();

        enemyProjectile.OnRallyStart();
    }





}
