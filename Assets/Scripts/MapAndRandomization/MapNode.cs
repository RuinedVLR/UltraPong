using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Map
{
    public class MapNode : MonoBehaviour, IPointerClickHandler
    {
        [Tooltip("Index of this node in the fixed map sequence (0, 1, 2, ...).")]
        [SerializeField] private int sequenceIndex;

        [Tooltip("If on takes fight from bossScenePool instead of enemyScenePool.")]
        [SerializeField] private bool isBossNode;

        [SerializeField] private ScenePool enemyScenePool;
        [SerializeField] private ScenePool bossScenePool;

        [Tooltip("Optional: visually reflects node availability (e.g. via CanvasGroup.alpha).")]
        [SerializeField] private CanvasGroup availabilityIndicator;
        [SerializeField] private Image nodeSprite;
        private bool subscribedToManager;

        private void OnEnable()
        {
            TrySubscribeAndRefresh();
        }

        private void OnDisable()
        {
            if (MapManager.Instance != null)
                MapManager.Instance.OnProgressChanged -= UpdateAvailabilityIndicator;
            subscribedToManager = false;
        }

        private void TrySubscribeAndRefresh()
        {
            if (MapManager.Instance == null)
            {
                // MapManager hasn't finished its Awake() yet (initialization order race on first scene load).
                // Retry next frame instead of silently staying in the wrong visual state.
                StartCoroutine(WaitForManagerThenRefresh());
                return;
            }

            if (!subscribedToManager)
            {
                MapManager.Instance.OnProgressChanged += UpdateAvailabilityIndicator;
                subscribedToManager = true;
            }

            UpdateAvailabilityIndicator();
        }

        private IEnumerator WaitForManagerThenRefresh()
        {
            while (MapManager.Instance == null)
                yield return null;

            TrySubscribeAndRefresh();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (MapManager.Instance == null)
            {
                Debug.LogError("MapManager instance not found in the scene.");
                return;
            }

            if (!MapManager.Instance.CanSelectNode(sequenceIndex))
            {
                Debug.Log($"Node \"{name}\" (index {sequenceIndex}) is not the next available node.");
                return;
            }

            LoadRandomScene();
            UpdateAvailabilityIndicator();
        }

        private void LoadRandomScene()
        {
            ScenePool pool = isBossNode ? bossScenePool : enemyScenePool;

            if (pool == null)
            {
                Debug.LogError($"MapNode \"{name}\" has no ScenePool assigned for {(isBossNode ? "boss" : "enemy")} nodes.");
                return;
            }

            string sceneName = pool.GetRandomSceneName();
            if (string.IsNullOrEmpty(sceneName))
                return;

            // Progress only advances once the battle is won — call MapManager.Instance.CompletePendingNode()
            // from your gameplay code (e.g. on victory) before returning to the map scene.
            MapManager.Instance.EnterNode(sequenceIndex);
            SceneManager.LoadScene(sceneName);
        }

        private void UpdateAvailabilityIndicator()
        {
            if (MapManager.Instance == null)
                return;

            bool completed = MapManager.Instance.IsNodeCompleted(sequenceIndex);
            bool selectable = MapManager.Instance.CanSelectNode(sequenceIndex);

            if (availabilityIndicator != null)
            {
                availabilityIndicator.interactable = selectable;
                availabilityIndicator.blocksRaycasts = selectable;
            }

            if (nodeSprite == null)
            {
                Debug.LogWarning($"MapNode \"{name}\" has no nodeSprite assigned, cannot update its appearance.");
                return;
            }

            Color color = completed ? Color.black : Color.white;
            color.a = completed || selectable ? 1f : 0.4f;
            nodeSprite.color = color;
        }
    }
}

