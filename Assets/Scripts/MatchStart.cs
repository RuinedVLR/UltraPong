using System.Collections;
using TMPro;
using UnityEngine;

public class MatchStart : MonoBehaviour
{
    [SerializeField] private string countTextName = "CountdownText";

    private BallManager enemyProjectile;
    private TextMeshProUGUI countText;
    public float countFloat = 4f;
    private int countInt;

    private void Start()
    {
        enemyProjectile = FindFirstObjectByType<BallManager>();
        countText = FindCountText();

        StartCoroutine(WaitForStart());
    }

    private TextMeshProUGUI FindCountText()
    {
        var obj = GameObject.Find(countTextName);

        if (obj == null)
        {
            Debug.LogError($"MatchStart could not find a GameObject named '{countTextName}' in the scene.");
            return null;
        }

        return obj.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (countText == null) return;
        if (countFloat <= 1.2) return;

        countFloat -= Time.deltaTime;
        countInt = (int)countFloat;
        countText.text = countInt.ToString();
    }

    public IEnumerator WaitForStart()
    {
        if (countText == null)
            yield break;

        countText.enabled = true;
        countFloat = 4f;
        yield return new WaitForSeconds(3);

        countText.text = "GO";
        yield return new WaitForSeconds(1);
        countText.enabled = false;
        enemyProjectile.OnRallyStart();
    }
}
