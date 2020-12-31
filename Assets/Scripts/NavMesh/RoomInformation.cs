using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pieter.NavMesh;
using System;
using Pieter.GraphTraversal;
using Pieter.Grid;
using UnityEditorInternal;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

public class RoomInformation : MonoBehaviour
{
    private int roomID = -1;

    public void SetID(int id)
    {
        roomID = id;
        worldSpaceTextDebug.Write("RoomInformation", roomID.ToString());
    }
    [SerializeField] private NavMeshGenerator meshGenerator = null;


    public NavMeshGenerator NavMeshGenerator => meshGenerator;
    [SerializeField] private Pieter.GraphTraversal.TraversalGenerator traversalGenerator = null;

    [SerializeField] private NavMeshHolder navMeshHolder = null;
    private AStarNavMeshNavigation navMeshNavigation = null;
    public AStarNavMeshNavigation NavMeshNavigation => navMeshNavigation;

    [SerializeField] private EntrancePoints entrancePoints = null;
    public EntrancePoints EntrancePoints => entrancePoints;

    public RoomGrid roomGrid = null;

    public bool showOccupiedSpace = false;
    public Vector3 center;
    [FormerlySerializedAs("helfExtents")] public Vector3 extents;

    private Dictionary<int, RoomInformation> connectedRooms = new Dictionary<int, RoomInformation>();
    [SerializeField] private FireGenerator fireGenerator = null;
    [SerializeField] private Vertex centerVertex = null;
    [SerializeField] private WorldSpaceTextDebug worldSpaceTextDebug = null;

    public Vector3 RoomCenter => centerVertex.Position;

    public bool IsOnFire => fireGenerator.IsOnFire;

    public void GetRotatedCenter(out Vector3 center, out Vector3 extents)
    {
        center = this.center;
        center = this.transform.rotation * center;
        extents = this.transform.rotation * (new Vector3(this.extents.x, this.extents.y, this.extents.z));
    }

    private void Awake()
    {
        navMeshHolder.AddNavMesh(meshGenerator);
        navMeshNavigation = new AStarNavMeshNavigation(navMeshHolder);
    }

    private void OnValidate()
    {
        if (meshGenerator != null)
        {
            meshGenerator.containedRoom = this;
        }

        if (traversalGenerator != null)
        {
            traversalGenerator.containedRoom = this;
        }

        navMeshHolder.ResetData();
        navMeshHolder.AddNavMesh(meshGenerator);
    }

    public NavMeshEntrance GetDoorForRoom(RoomInformation roomInformation)
    {
        for (int i = 0; i < GetEntrances.Length; i++)
        {
            if(GetEntrances[i].ID == roomInformation.ID)
            {
                return GetEntrances[i];
            }
        }
        return null;
    }

    public void StartFire()
    {
        fireGenerator.StartFire(this, 0);
    }

    public void StartFire(int TriangleID)
    {
        if (fireGenerator.CanBurn)
        {
            fireGenerator.StartFire(this, TriangleID);
        }
    }

    public RoomInformation GetConnectedRoomFromEntranceWithID(int connectedEntranceID)
    {
        return connectedRooms[connectedEntranceID];
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }

        RoomInformation room = (RoomInformation) other;
        return room.roomID == roomID;
    }

    public Pieter.GraphTraversal.TraversalGenerator TraversalGenerator => traversalGenerator;

    public NavMeshEntrance GetRandomEntrance()
    {
        return meshGenerator.GetRandomGenerator;
    }
    public NavMeshEntrance GetEntrance(int index)
    {
        return meshGenerator.GetEntrance(index);
    }

    public NavMeshEntrance[] GetEntrances => meshGenerator.Entrances;

    public int ID => roomID;

    public RoomInformation[] GetConnectedRooms => connectedRooms.Values.ToArray();

    public Vertex GetCenterVertex => centerVertex;

    public Vector3 GetRandomSpotInsideRoom => navMeshHolder.GetRandomPointInTriangle(navMeshHolder.GetRandomTriangle());


    private void OnDrawGizmos()
    {
        if (showOccupiedSpace)
        {
            Gizmos.color = Color.yellow;
            Vector3 cent;
            Vector3 ext;
            GetRotatedCenter(out cent, out ext);

            Gizmos.DrawCube(this.transform.position + cent, ext) ;

            Gizmos.color = Color.magenta;
            Vector3 corner1 = this.transform.position + cent - ext / 2;
            Vector3 corner2 = this.transform.position + cent + ext / 2;

            Vector3 adjustedCorner1 = new Vector3(Mathf.Min(corner1.x, corner2.x), Mathf.Min(corner1.y, corner2.y), Mathf.Min(corner1.z, corner2.z));
            Vector3 adjustedCorner2 = new Vector3(Mathf.Max(corner1.x, corner2.x), Mathf.Max(corner1.y, corner2.y), Mathf.Max(corner1.z, corner2.z));

            Gizmos.DrawSphere(adjustedCorner1, 0.4f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(adjustedCorner2, 0.4f);
        }
    }

    public void AddConnectedRoom(RoomInformation containedRoom, NavMeshEntrance entrance)
    {
        try
        {
            Debug.Log("Adding connected room information in room: " + ID + " connected to room: " + containedRoom.ID + " by entrance " + entrance.ID);
            connectedRooms.Add(entrance.ID, containedRoom);
        }
        catch(Exception ex)
        {
            Debug.Log("Connected Room Failed " + this.name + " to " + containedRoom.name + " from entrance " + entrance.ID);
            Debug.Log("Error: " + ex.Message);
        }
    }
    public bool IsInsideRoomArea(Vector3 position)
    {
        Vector3 cent;
        Vector3 ext;
        GetRotatedCenter(out cent, out ext);
        Vector3 point1 = this.transform.position + cent - ext / 2;
        Vector3 point2 = this.transform.position + cent + ext / 2;
        Vector3 adjustedCorner1 = new Vector3(Mathf.Min(point1.x, point2.x), Mathf.Min(point1.y, point2.y), Mathf.Min(point1.z, point2.z));
        Vector3 adjustedCorner2 = new Vector3(Mathf.Max(point1.x, point2.x), Mathf.Max(point1.y, point2.y), Mathf.Max(point1.z, point2.z));

        return position.x > adjustedCorner1.x && position.x < adjustedCorner2.x &&
                position.y > adjustedCorner1.y && position.y < adjustedCorner2.y &&
                position.z > adjustedCorner1.z && position.z < adjustedCorner2.z;
    }

    public override int GetHashCode()
    {
        int hashCode = 780059485;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + roomID.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(centerVertex);
        return hashCode;
    }
}
