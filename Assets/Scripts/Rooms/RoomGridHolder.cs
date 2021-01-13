using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Pieter.Grid
{
    [CreateAssetMenu(menuName = "LevelGeneration/RoomGridHolder")]
    public class RoomGridHolder : ScriptableObject
    {
        public List<RoomGrid> rooms = new List<RoomGrid>();

        public void Reset()
        {
            rooms = new List<RoomGrid>();
        }
        public void AddRoom(RoomGrid grid)
        {
            rooms.Add(grid);
        }

        public void RemoveRoom(RoomGrid grid)
        {
            rooms.Remove(grid);
        }

        public RoomGrid FindRoomById(int ID)
        {
            foreach (RoomGrid room in rooms)
            {
                if(room.ID == ID)
                {
                    return room;
                }
            }

            return null;
        }

        public GridPoint GetGridPoint(Vector2Int fireStartPoint, RoomInformation room)
        {
            return FindRoomById(room.ID).GetGridPoint(fireStartPoint);
        }

    }
}