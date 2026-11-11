using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    public class MapScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IScrollHandler
    {
        public float tweenBackDuration = 0.3f;
        public Ease tweenBackEase;
        public bool freezeX;
        public FloatMinMax xConstraints = new FloatMinMax();
        public bool freezeY;
        public FloatMinMax yConstraints = new FloatMinMax();
        public float scrollSpeed = 500f;
        public float dragSpeedMultiplier = 1f;
        private Vector2 offset;
        private Vector3 lastMousePos;
        private float zDisplacement;
        private bool dragging;
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            EnsureCamera();
        }

        private void OnEnable()
        {
            EnsureCamera();
        }

        private void EnsureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("MapScroll could not find a camera (Camera.main is null).");
                return;
            }

            zDisplacement = -mainCamera.transform.position.z + transform.position.z;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Mouse Down");
            lastMousePos = MouseInWorldCoords();
            transform.DOKill();
            dragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Mouse Up");
            dragging = false;
            TweenBack();
        }

        public void OnScroll(PointerEventData eventData)
        {
            transform.DOKill();
            Vector2 scrollDelta = eventData.scrollDelta * scrollSpeed * Time.deltaTime;
            transform.position = new Vector3(
                freezeX ? transform.position.x : transform.position.x + -scrollDelta.x,
                freezeY ? transform.position.y : transform.position.y + -scrollDelta.y,
                transform.position.z);
            TweenBack();
        }

        private void Update()
        {
            if (!dragging) return;

            Vector3 mousePos = MouseInWorldCoords();
            Vector3 mouseDelta = mousePos - lastMousePos;
            lastMousePos = mousePos;
            //Debug.Log(mousePos);
            transform.position = new Vector3(
                freezeX ? transform.position.x : transform.position.x + mouseDelta.x * dragSpeedMultiplier,
                freezeY ? transform.position.y : transform.position.y + mouseDelta.y * dragSpeedMultiplier,
                transform.position.z);
        }

        // returns mouse position in World coordinates for GameObject to follow. 
        private Vector3 MouseInWorldCoords()
        {
            EnsureCamera();
            if (mainCamera == null)
                return transform.position;

            Vector3 screenMousePos = Input.mousePosition;
            //Debug.Log(screenMousePos);
            screenMousePos.z = zDisplacement;
            return mainCamera.ScreenToWorldPoint(screenMousePos);
        }

        private void TweenBack()
        {
            if (freezeY)
            {
                if (transform.localPosition.x >= xConstraints.min && transform.localPosition.x <= xConstraints.max)
                    return;

                float targetX = transform.localPosition.x < xConstraints.min ? xConstraints.min : xConstraints.max;
                transform.DOLocalMoveX(targetX, tweenBackDuration).SetEase(tweenBackEase);
            }
            else if (freezeX)
            {
                if (transform.localPosition.y >= yConstraints.min && transform.localPosition.y <= yConstraints.max)
                    return;

                float targetY = transform.localPosition.y < yConstraints.min ? yConstraints.min : yConstraints.max;
                transform.DOLocalMoveY(targetY, tweenBackDuration).SetEase(tweenBackEase);
            }
        }
    }
}
