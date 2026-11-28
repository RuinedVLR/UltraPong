using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    /// <summary>
    /// Keeps track of progress through the current run's fixed, single-path map.
    /// Persists across scene loads (map -> fight -> map) for the duration of the run.
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [Tooltip("Scene name of the map — the canvas this script lives on is only active while this scene is loaded.")]
        [SerializeField] private string mapSceneName = "Map";

        [Tooltip("Index of the last completed node. -1 means no node has been completed yet, so only node 0 can be selected.")]
        [SerializeField] private int currentNodeIndex = -1;

        [Tooltip("Index of the node whose battle scene is currently loaded (awaiting completion). -1 means none.")]
        [SerializeField] private int pendingNodeIndex = -1;

        private string pendingSceneName;

        public int CurrentNodeIndex => currentNodeIndex;
        public int PendingNodeIndex => pendingNodeIndex;
        public string PendingSceneName => pendingSceneName;

        /// <summary>
        /// Raised whenever progress changes, so map nodes can refresh their availability indicators.
        /// </summary>
        public event Action OnProgressChanged;

        /// <summary>
        /// Raised when the player enters a battle (before the scene loads), with the node index and
        /// the scene name that was rolled for it. Use this as the save point.
        /// </summary>
        public event Action<int, string> OnNodeEntered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // The subscription survives gameObject.SetActive(false) below, since it isn't tied to Unity's enabled state.
            SceneManager.sceneLoaded += HandleSceneLoaded;
            UpdateCanvasVisibility(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateCanvasVisibility(scene.name);
        }

        private void UpdateCanvasVisibility(string sceneName)
        {
            gameObject.SetActive(sceneName == mapSceneName);
        }

        /// <summary>
        /// Returns true if the node with the given sequence index is the next one allowed to be selected.
        /// </summary>
        public bool CanSelectNode(int nodeSequenceIndex)
        {
            return nodeSequenceIndex == currentNodeIndex + 1;
        }

        public bool IsNodeCompleted(int nodeSequenceIndex)
        {
            return nodeSequenceIndex <= currentNodeIndex;
        }

        /// <summary>
        /// Call this when the player enters a node's fight scene, before loading it.
        /// Does NOT advance progress yet — progress only advances once the fight is won (see CompletePendingNode).
        /// </summary>
        public bool EnterNode(int nodeSequenceIndex, string sceneName)
        {
            if (!CanSelectNode(nodeSequenceIndex))
                return false;

            pendingNodeIndex = nodeSequenceIndex;
            pendingSceneName = sceneName;
            OnNodeEntered?.Invoke(pendingNodeIndex, pendingSceneName);
            return true;
        }

        /// <summary>
        /// Call this from gameplay code when the player wins the fight for the currently pending node.
        /// Advances progress so the next node on the map becomes available.
        /// </summary>
        public bool CompletePendingNode()
        {
            if (pendingNodeIndex < 0 || !CanSelectNode(pendingNodeIndex))
                return false;

            currentNodeIndex = pendingNodeIndex;
            pendingNodeIndex = -1;
            pendingSceneName = null;
            OnProgressChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Marks the node as completed and advances progress directly, if it is the expected next node.
        /// Useful for quick testing without a full gameplay win/lose flow.
        /// </summary>
        public bool TryCompleteNode(int nodeSequenceIndex)
        {
            if (!CanSelectNode(nodeSequenceIndex))
                return false;

            currentNodeIndex = nodeSequenceIndex;
            OnProgressChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Resets progress, e.g. when starting a brand new run.
        /// </summary>
        public void ResetRun()
        {
            currentNodeIndex = -1;
            pendingNodeIndex = -1;
            pendingSceneName = null;
            OnProgressChanged?.Invoke();
        }

        /// <summary>
        /// Directly restores progress from a save file, without the "next node" validation
        /// used by TryCompleteNode/CompletePendingNode. Also restores a pending (unfinished) battle, if any.
        /// </summary>
        public void SetProgress(int nodeIndex, int pendingIndex = -1, string pendingScene = null)
        {
            currentNodeIndex = nodeIndex;
            pendingNodeIndex = pendingIndex;
            pendingSceneName = pendingScene;
            OnProgressChanged?.Invoke();
        }

        public void ReturnToMap(string mapSceneName)
        {
            SceneManager.LoadScene(mapSceneName);
        }
    }
}
