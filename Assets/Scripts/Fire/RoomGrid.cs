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
        public RoomGrid roomgrid;
        public Vector3 center;
        public Vector2Int gridPosition;
        public Vector2Int[] adjacentPointsXY;
        public RoomGrid adjacentRoom;
        public Vector2Int adjacentRoomPointXY;

        //Fire information
        public bool isOnFire = false;
        public float hp = 40;

        // Entrance information
        public NavMeshEntrance entrance;
        public bool isEntrance;

        public bool isInside;

        public GridPoint GetAjacentRoomGridPoint()
        {
            return adjacentRoom.GetGridPoint(adjacentRoomPointXY);
        }


        public GridPoint[] GetAllAdjacentGridPoints()
        {
            List<GridPoint> adjPoints = new List<GridPoint>();
            foreach (Vector2Int adjPoint in this.adjacentPointsXY)
            {
                adjPoints.Add(roomgrid.GetGridPoint(adjPoint));
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

        public override string ToString()
        {
            return gridPosition.ToString() + " Ent? " + isEntrance;
        }
    }
    public struct AdjacentRoomGridInfo
    {
        public int entranceId;
        public int exitId;
        public RoomGrid room;
    }
    [RequireComponent(typeof(RoomInformation))]
    public class RoomGrid : MonoBehaviour
    {
        private RoomInformation roomInfo = null;
        public int ID => roomInfo.ID;

        public int GetTotalGridPoints => grid.Length;

        public int GetGridWidth => grid.GetLength(0);

        private Vector2Int[] entrancePoints = new Vector2Int[0];
        private int[] entranceIds = null;
        [SerializeField] private float tileSize = 1;
        [SerializeField] private LayerMask outlineMask = 0; 
        private GridPoint[,] grid = new GridPoint[0, 0];

        private List<AdjacentRoomGridInfo> toAddAdjacentRooms = new List<AdjacentRoomGridInfo>();
        private bool hasBeenInit = false;
        RaycastHit[] hits = new RaycastHit[50];

        public void Initialize(Vector3 position, Quaternion rotation)
        {
            roomInfo = GetComponent<RoomInformation>();

            float x = roomInfo.extents.x;
            float z = roomInfo.extents.z;

            int numberOfXTiles = Mathf.CeilToInt(x / tileSize);
            int numberOfZTiles = Mathf.CeilToInt(z / tileSize);

            grid = new GridPoint[numberOfXTiles, numberOfZTiles];

            Quaternion rot = rotation;
            Vector3 transPos = roomInfo.center - roomInfo.extents/2;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Vector3 pos = transPos + rot * new Vector3((i * tileSize) + tileSize / 2, this.transform.position.y, (j * tileSize) + tileSize / 2);
                    grid[i, j] = new GridPoint();
                    grid[i, j].roomgrid = this;
                    grid[i, j].center = position + pos;
                    grid[i, j].gridPosition = new Vector2Int(i, j);
                    grid[i, j].adjacentPointsXY = GetAdjacentXY(i, j, grid.GetLength(0), grid.GetLength(1));
                    int hitCount = Physics.RaycastNonAlloc(pos+Vector3.up*10, Vector3.down, hits, 30, outlineMask);
                    bool res = false;
                    for (int k = 0; k < hitCount; k++)
                    {
                        if (hits[k].collider.transform.parent.parent.gameObject.Equals(this.gameObject))
                        {
                            res = true;
                            break;
                        }
                    }
                    grid[i, j].isInside = res;
                }
            }
            entrancePoints = new Vector2Int[roomInfo.GetEntrances.Length];
            entranceIds = new int[roomInfo.GetEntrances.Length];
            for (int i = 0; i < roomInfo.GetEntrances.Length; i++)
            {
                Vector2Int entrancePos = FindGridPointClosestToPosition(roomInfo.GetEntrance(i).entrance.transform.position);//( roomInfo.GetEntrance(i).entrance.transform.localPosition) / tileSize;

                grid[entrancePos.x, entrancePos.y].entrance = roomInfo.GetEntrance(i);
                grid[entrancePos.x, entrancePos.y].isEntrance = true;
                entrancePoints[i] = new Vector2Int(entrancePos.x, entrancePos.y);
                entranceIds[i] = roomInfo.GetEntrance(i).ID;
            }
            for (int i = toAddAdjacentRooms.Count-1; i >= 0; i--)
            {
                AddGrid(toAddAdjacentRooms[i]);
                toAddAdjacentRooms.RemoveAt(i);
            }
            hasBeenInit = true;
        }

        public Vector2Int GetIntPos(Vector3 fireStartPos)
        {
            return FindGridPointClosestToPosition(fireStartPos);
        }

        private Vector2Int FindGridPointClosestToPosition(Vector3 pos)
        {
            float closest = SqrDistance(grid[0, 0].center, pos);
            float testClosest = 0;
            Vector2Int position = new Vector2Int(0, 0);
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    testClosest = SqrDistance(grid[i, j].center, pos);
                    if(testClosest < closest)
                    {
                        closest = testClosest;
                        position = new Vector2Int(i, j);
                    }
                }
            }
            return position;
        }

        private float SqrDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2);
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
            if (hasBeenInit)
            {
                AddGrid(new AdjacentRoomGridInfo() { entranceId = idRoom1, exitId = idRoom2, room = nextRoomGrid });
            }
            else
            {
                toAddAdjacentRooms.Add(new AdjacentRoomGridInfo() { entranceId = idRoom1, exitId = idRoom2, room = nextRoomGrid });
            }
        }

        private void AddGrid(AdjacentRoomGridInfo info)
        {
            Vector2Int room1GridPoint = GetGridPointFromEntranceID(info.entranceId);
            if (room1GridPoint.x < 0 || room1GridPoint.y < 0)
            {
                return;
            }

            Vector2Int room2GridPoint = info.room.GetGridPointFromEntranceID(info.exitId);
            if (room2GridPoint.x < 0 || room2GridPoint.y < 0)
            {
                return;
            }

            grid[room1GridPoint.x, room1GridPoint.y].adjacentRoom = info.room;
            grid[room1GridPoint.x, room1GridPoint.y].adjacentRoomPointXY = room2GridPoint;
        }

        public Vector2Int GetGridPointFromEntranceID(int id)
        {
            for (int i = 0; i < entrancePoints.Length; i++)
            {
                if (entranceIds[i] == id)
                {
                    return entrancePoints[i];
                }
            }

            return new Vector2Int(-1, -1);
        }

        public GridPoint GetGridPoint(Vector2Int gridPointAdjacentRoomPointXy)
        {
            return grid[gridPointAdjacentRoomPointXy.x, gridPointAdjacentRoomPointXy.y];
        }

        public GridPoint GetRandomGridPoint()
        {
            return grid[Random.Range(0, grid.GetLength(0)), Random.Range(0, grid.GetLength(1))];
        }

        private void OnDrawGizmos()
        {
            foreach (GridPoint gridPoint in grid)
            {
                if(gridPoint == null)
                {
                    continue;
                }
                Gizmos.color = gridPoint.isInside ? Color.green : Color.yellow;
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
    }
}
