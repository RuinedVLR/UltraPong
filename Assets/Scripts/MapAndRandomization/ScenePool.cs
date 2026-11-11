using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "ScenePool", menuName = "Map/Scene Pool")]
    public class ScenePool : ScriptableObject
    {
        [Tooltip("Scene names must be added to Build Settings (File > Build Settings).")]
        public List<string> sceneNames = new List<string>();

        public string GetRandomSceneName()
        {
            if (sceneNames == null || sceneNames.Count == 0)
            {
                Debug.LogError($"ScenePool \"{name}\" is empty, cannot pick a random scene.");
                return null;
            }

            int index = Random.Range(0, sceneNames.Count);
            return sceneNames[index];
        }
    }
}
