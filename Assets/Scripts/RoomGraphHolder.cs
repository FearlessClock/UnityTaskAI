﻿using Pieter.NavMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="LevelGeneration/RoomGraphHolder")]
public class RoomGraphHolder : ScriptableObject
{
    public Node[] rooms;

    public void AddRoom(RoomInformation room)
    {
        if(rooms == null)
        {
            rooms = new Node[0];
        }
        Node nextRoom = new Node();
        nextRoom.payload = room;
        nextRoom.id = room.ID;
        Node[] roomsNew = new Node[rooms.Length + 1];
        for (int i = 0; i < rooms.Length; i++)
        {
            roomsNew[i] = rooms[i];
        }
        roomsNew[roomsNew.Length - 1] = nextRoom;
        rooms = roomsNew;
    }

    public void AddChild(RoomInformation selectedRoom, RoomInformation child)
    {
        Node selectedNode = null;
        for (int i = 0; i < rooms.Length; i++)
        {
            if((rooms[i].payload as RoomInformation).ID == selectedRoom.ID)
            {
                selectedNode = rooms[i];
            }
        }

        for (int i = 0; i < rooms.Length; i++)
        {
            if ((rooms[i].payload as RoomInformation).ID == child.ID)
            {
                selectedNode.AddNode(rooms[i]);
                rooms[i].AddNode(selectedNode);
            }
        }
    }

    public void Clear()
    {
        rooms = null;
    }

    public RoomInformation FindRoomFromStartMatching(System.Func<int, bool> predicate, int id)
    {
        Node room = GetRoom(id);
        if(room == null)
        {
            return null;
        }

        List<int> foundNodes = new List<int>();
        Node node = DFSSearch(predicate, foundNodes, room, 0);
        if (node != null)
        {
            return node.payload as RoomInformation;
        }
        else
        {
            return null;
        }
    }

    private Node DFSSearch(System.Func<int, bool> predicate, List<int> foundNodes, Node currentNode, int depth)
    {
        foundNodes.Add(currentNode.id);

        if(predicate(depth))
        {
            return currentNode;
        }
        if(depth > 400)
        {
            Debug.Log("There is a problem");
            return null;
        }
        for (int i = 0; i < currentNode.connectedNodes.Length; i++)
        {
            bool isOnFire = (currentNode.connectedNodes[i].payload as RoomInformation).IsOnFire;
            NavMeshEntrance entranceToConnectedRoom = (currentNode.connectedNodes[i].payload as RoomInformation).GetDoorForRoom(currentNode.payload as RoomInformation);
            bool isPassable = true;//entranceToConnectedRoom != null ? entranceToConnectedRoom.IsPassable : true;
            if (!foundNodes.Contains(currentNode.connectedNodes[i].id) && !isOnFire && isPassable)
            {
                return DFSSearch(predicate, foundNodes, currentNode.connectedNodes[i], ++depth);
            }
        }
        // Found nothing
        if((currentNode.payload as RoomInformation).IsOnFire)
        {
            return null;
        }
        else
        {
            return currentNode;
        }
    }


    private Node GetRoom(int id)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if((rooms[i].payload as RoomInformation).ID == id)
            {
                return rooms[i];
            }
        }
        return null;
    }

    public void DebugGraph()
    {
        if (Application.isPlaying)
        {
            foreach (Node node in rooms)
            {
                RoomInformation roomInfo = (RoomInformation)node.payload;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(roomInfo.GetCenterVertex.Position, 0.5f);
                foreach (Node child in node.connectedNodes)
                {
                    RoomInformation childRoomInfo = (RoomInformation)child.payload;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(childRoomInfo.GetCenterVertex.Position, roomInfo.GetCenterVertex.Position);
                }
            }
        }
    }
}

[System.Serializable]
public class Node
{
    public int id;
    [HideInInspector]
    public Object payload;
    public Node[] connectedNodes = new Node[0];

    public void AddNode(Node graph)
    {
        if (connectedNodes == null)
        {
            connectedNodes = new Node[0];
        }
        Node[] graphNew = new Node[connectedNodes.Length + 1];
        for (int i = 0; i < connectedNodes.Length; i++)
        {
            graphNew[i] = connectedNodes[i];
        }
        graphNew[graphNew.Length - 1] = graph;
        connectedNodes = graphNew;
    }
}
