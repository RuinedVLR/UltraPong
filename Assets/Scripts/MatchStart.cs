using System.Collections;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchStart : MonoBehaviour
{
    private EnemyProjectile enemyProjectile;
    public TextMeshProUGUI countText;
    public float countFloat = 4f;
    private int countInt;

    private void Start()
    {
        if (gameObject != null) DontDestroyOnLoad(gameObject);
        enemyProjectile = GameObject.Find("Enemy").GetComponent<EnemyProjectile>();

        StartCoroutine(WaitForStart());
    }


    private void Update()
    {
        if (countFloat <= 1.2) return;
        countFloat -= Time.deltaTime;
        countInt = (int)countFloat;
        countText.text = countInt.ToString();
    }

     private IEnumerator WaitForStart()
    {
        yield return new WaitForSeconds(3);

        countText.text = "GO";
        yield return new WaitForSeconds(1);
        countText.enabled = false;
        enemyProjectile.OnRallyStart();
        
    }

}
