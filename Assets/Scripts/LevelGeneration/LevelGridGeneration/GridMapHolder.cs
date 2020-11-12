using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "LevelGeneration/GridMapHolder", order = 5)]
public class GridMapHolder : ScriptableObject
{
    public GridWorldMap gridWorldMap = null;

    public RoomInformation GetRoomAtWorldPosition(Vector3 position)
    {
        Vector2Int vec = GetGridPosition(position);
        return gridWorldMap.At(new Vector2(vec.x, vec.y));
    }

    public Vector2Int GetGridPosition(Vector3 position)
    {
        position /= gridWorldMap.tileSize;
        Vector2Int updatedPosition = new Vector2Int(0, 0);
        if (position.x < 0 && position.z >= 0)
        {
            float X = position.x;
            float Y = position.z;
            if (position.x < 0)
            {
                X = Mathf.FloorToInt(position.x);
            }
            updatedPosition = new Vector2Int((int)X, (int)Y);
        }
        else if (position.x >= 0 && position.z >= 0)
        {
            updatedPosition = new Vector2Int((int)position.x, (int)position.z);
        }
        else if (position.x < 0 && position.z < 0)
        {
            float X = position.x;
            float Y = position.z;
            if (position.x < 0)
            {
                X = Mathf.FloorToInt(position.x);
            }
            if (position.z < 0)
            {
                Y = Mathf.FloorToInt(position.z);
            }
            updatedPosition = new Vector2Int((int)X, (int)Y);
        }
        else if (position.x >= 0 && position.z < 0)
        {
            float X = position.x;
            float Y = position.z;
            if (position.z < 0)
            {
                Y = Mathf.FloorToInt(position.z);
            }
            updatedPosition = new Vector2Int((int)X, (int)Y);
        }
        return updatedPosition;
    }
}