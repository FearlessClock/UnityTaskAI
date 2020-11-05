using System;
using System.Collections;
using System.Collections.Generic;
using Pieter.NavMesh;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pieter.Grid
{
    public class GridPoint
    {
        public RoomGrid room;
        public Vector3 center;
        public Vector2Int[] adjacentPointsXY;
        public RoomGrid adjacentRoom;
        public Vector2Int adjacentRoomPointXY;

        //Fire information
        public bool isOnFire = false;
        public float hp = 40;

        // Entrance information
        public NavMeshEntrance entrance;
        public bool isEntrance;

        public GridPoint GetAjacentRoomGridPoint()
        {
            return adjacentRoom.GetGridPoint(adjacentRoomPointXY);
        }


        public GridPoint[] GetAllAdjacentGridPoints()
        {
            List<GridPoint> adjPoints = new List<GridPoint>();
            foreach (Vector2Int vector2Int in this.adjacentPointsXY)
            {
                adjPoints.Add(room.GetGridPoint(vector2Int));
            }

            if (this.adjacentRoom != null && isEntrance && entrance.IsOpen)
            {
                if (this.GetAjacentRoomGridPoint() != null)
                {
                    adjPoints.Add(this.GetAjacentRoomGridPoint());
                }
            }
            return adjPoints.ToArray();
        }
    }

    [RequireComponent(typeof(RoomInformation))]
    public class RoomGrid : MonoBehaviour
    {
        private RoomInformation roomInfo = null;
        private Vector2Int[] entrancePoints = null;
        private int[] entranceIds = null;
        [SerializeField] private float tileSize = 1;
        private GridPoint[,] grid = new GridPoint[0, 0];

        private void Awake()
        {
            roomInfo = GetComponent<RoomInformation>();

            // The extents are actaully halfExtents
            float x = roomInfo.extents.x;
            float z = roomInfo.extents.z;

            int numberOfXTiles = Mathf.CeilToInt(x / tileSize);
            int numberOfZTiles = Mathf.CeilToInt(z / tileSize);

            grid = new GridPoint[numberOfXTiles, numberOfZTiles];
            Quaternion rot = this.transform.rotation;
            Vector3 transPos = this.transform.position;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = new GridPoint();
                    grid[i, j].room = this;
                    Vector3 pos = transPos + rot * new Vector3((i * tileSize) + tileSize / 2, 0, (j * tileSize) + tileSize / 2);
                    grid[i,j].center = pos;
                    grid[i, j].adjacentPointsXY = GetAdjacentXY(i, j, grid.GetLength(0), grid.GetLength(1));
                }
            }
            entrancePoints = new Vector2Int[roomInfo.GetEntrances.Length];
            entranceIds = new int[roomInfo.GetEntrances.Length];
            for (int i = 0; i < roomInfo.GetEntrances.Length; i++)
            {
                Vector3 entrancePos = ( roomInfo.GetEntrance(i).entranceMidPoint.transform.localPosition) / tileSize;
                int xPos = Mathf.FloorToInt(entrancePos.x);
                if(xPos >= grid.GetLength(0))
                {
                    xPos = grid.GetLength(0) - 1;
                }
                else if (xPos < 0)
                {
                    xPos = 0;
                }

                int zPos = Mathf.FloorToInt(entrancePos.z);
                if (zPos >= grid.GetLength(1))
                {
                    zPos = grid.GetLength(1) - 1;
                }
                else if (zPos < 0)
                {
                    zPos = 0;
                }

                grid[xPos, zPos].entrance = roomInfo.GetEntrance(i);
                grid[xPos, zPos].isEntrance = true;
                entrancePoints[i] = new Vector2Int(xPos, zPos);
                entranceIds[i] = roomInfo.GetEntrance(i).ID;
            }
        }

        private void Start()
        {
            Awake();
        }

        private Vector2Int[] GetAdjacentXY(int x, int z, int gridMaxX, int gridMaxZ)
        {
            List<Vector2Int> adjPoints = new List<Vector2Int>();
            Vector2Int[] adjPremade = new[]
                {new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1)};

            for (int k = 0; k < adjPremade.Length; k++)
            {
                if (adjPremade[k].x + x >= 0 && adjPremade[k].x + x < gridMaxX && adjPremade[k].y + z >= 0 && adjPremade[k].y + z < gridMaxZ)
                {
                    adjPoints.Add(new Vector2Int(adjPremade[k].x + x, adjPremade[k].y + z));
                }
            }

            return adjPoints.ToArray();
        }

        public void AddGrid(RoomGrid nextRoomGrid, int idRoom1, int idRoom2)
        {
            Vector2Int room1GridPoint = GetGridPointFromEntranceID(idRoom1);
            if (room1GridPoint.x < 0 || room1GridPoint.y < 0)
            {
                return;
            }

            Vector2Int room2GridPoint = nextRoomGrid.GetGridPointFromEntranceID(idRoom2);
            if (room2GridPoint.x < 0 || room2GridPoint.y < 0)
            {
                return;
            }

            grid[room1GridPoint.x, room1GridPoint.y].adjacentRoom = nextRoomGrid;
            grid[room1GridPoint.x, room1GridPoint.y].adjacentRoomPointXY = room2GridPoint;

        }

        public Vector2Int GetGridPointFromEntranceID(int id)
        {
            for (int i = 0; i < entrancePoints.Length; i++)
            {
                if (entranceIds[i] == id)
                {
                    return entrancePoints[id];
                }
            }

            return new Vector2Int(-1, -1);
        }

        private void OnDrawGizmos()
        {
            foreach (GridPoint gridPoint in grid)
            {
                //Gizmos.DrawWireCube(gridPoint.center, (tileSize * Vector3.one) - (Vector3.one * 0.1f));
                foreach (Vector2Int vector2Int in gridPoint.adjacentPointsXY)
                {
                    Gizmos.DrawLine(gridPoint.center, grid[vector2Int.x, vector2Int.y].center);
                }

                if (gridPoint.adjacentRoom != null)
                {
                    Gizmos.DrawLine(gridPoint.center, gridPoint.GetAjacentRoomGridPoint().center);
                }
            }

            Gizmos.color = Color.blue;
            if (entrancePoints != null)
            {
                foreach (Vector2Int entrancePoint in entrancePoints)
                {
                    Gizmos.DrawWireCube(grid[entrancePoint.x, entrancePoint.y].center, (tileSize * Vector3.one) - (Vector3.one * 0.2f));
                }
            }
        }

        public GridPoint GetGridPoint(Vector2Int gridPointAdjacentRoomPointXy)
        {
            return grid[gridPointAdjacentRoomPointXy.x, gridPointAdjacentRoomPointXy.y];
        }

        public GridPoint GetRandomGridPoint()
        {
            return grid[Random.Range(0, grid.GetLength(0)), Random.Range(0, grid.GetLength(1))];
        }

    }
}
