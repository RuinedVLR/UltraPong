using Map;
using UnityEngine;
using UnityEngine.UI;

public class ButtonConnection : MonoBehaviour
{
    [SerializeField] private Button button;

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        MapManager.Instance.ReturnToMap("Test_MapRandomization");
    }
}
