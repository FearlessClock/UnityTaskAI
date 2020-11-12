using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Pieter.NavMesh;
using UnityEngine;

public class EntrancePoints : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator generator = null;
    [SerializeField] private NavMeshEntrance[] entrancePoints = null;
    private enum Direction { up, down, left, right};
    private Dictionary<Direction, NavMeshEntrance> doorDirections = new Dictionary<Direction, NavMeshEntrance>();
    private List<Vector2Int> directions = new List<Vector2Int>();
    public Vector2Int[] Directions => directions.ToArray();

    private void Awake()
    {
        UpdateEntranceValues();
    }

    public void UpdateEntranceValues()
    {
        UpdateEntranceDoorDirections();
    }

    public int Length
    {
        get { return entrancePoints.Length; }
    }

    public NavMeshEntrance[] Entrances
    {
        get { return entrancePoints; }
    }


    public NavMeshEntrance GetEntrance(int i)
    {
        return this.entrancePoints[i];
    }

    public NavMeshEntrance GetEntranceFromDirection(Vector2 direction)
    {
        if (direction.Equals(Vector2.right) && doorDirections.ContainsKey(Direction.right))
        {
            return doorDirections[Direction.right];
        }
        else if (direction.Equals(Vector2.left) && doorDirections.ContainsKey(Direction.left))
        {
            return doorDirections[Direction.left];
        }
        else if (direction.Equals(Vector2.up) && doorDirections.ContainsKey(Direction.up))
        {
            return doorDirections[Direction.up];
        }
        else if (direction.Equals(Vector2.down) && doorDirections.ContainsKey(Direction.down))
        {
            return doorDirections[Direction.down];
        }
        return null;
    }

    public void UpdateEntranceDoorDirections()
    {
        doorDirections.Clear();
        directions.Clear();
        foreach (NavMeshEntrance entrance in entrancePoints)
        {
            Debug.DrawLine(entrance.entrance.Position + Vector3.up * 2, generator.containedRoom.center + Vector3.up * 2, Color.white, 3);
            Vector2 direction = new Vector2((entrance.entrance.Position - generator.containedRoom.center).x, (entrance.entrance.Position - generator.containedRoom.center).z);

            direction.Normalize();
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction = new Vector2(1 * Mathf.Sign(direction.x), 0);
            }
            else
            {
                direction = new Vector2(0, 1 * Mathf.Sign(direction.y));
            }

            if (direction.Equals(Vector2.right))
            {
                doorDirections.Add(Direction.right, entrance);
                directions.Add(Vector2Int.right);
            }
            else if(direction.Equals(Vector2.left))
            {
                doorDirections.Add(Direction.left, entrance);
                directions.Add(Vector2Int.left);
            }
            else if(direction.Equals(Vector2.up))
            {
                doorDirections.Add(Direction.up, entrance);
                directions.Add(Vector2Int.up);
            }
            else if (direction.Equals(Vector2.down))
            {
                doorDirections.Add(Direction.down, entrance);
                directions.Add(Vector2Int.down);
            }

            entrance.entrance.SetDoorController(entrance.GetDoorController);
            entrance.generator = generator;
        }
    }

    public bool HasEntranceInDirection(Vector2 dir)
    {
        return GetEntranceFromDirection(dir) != null;
    }
}
