using UnityEngine;

public class PaddleTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float distanceFromCamera = 7f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        mouseScreenPos.z = distanceFromCamera;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        transform.position = mouseWorldPos;
    }
}
