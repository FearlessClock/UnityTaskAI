using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelGeneration/Room Prefab List")]
public class RoomPrefabsList : ScriptableObject
{
    public GridLevelSquareInformation[] gridLevels = null;

    public int Length => gridLevels.Length;
}
