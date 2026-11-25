using UnityEngine;

/// <summary>
/// Place on a root object in a battle scene to mark it as a boss fight.
/// GameManager looks for this on scene load to decide the win flow.
/// </summary>
public class BattleSceneInfo : MonoBehaviour
{
    public bool isBossBattle;
}