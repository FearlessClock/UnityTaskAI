using Pieter.NavMesh;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FireGenerator : MonoBehaviour
{
    [SerializeField] private Mesh mesh = null;
    [SerializeField] private MeshFilter filler = null;
    [SerializeField] private new ParticleSystem particleSystem = null;
    [SerializeField] private new ParticleSystem smokeParticleSystem = null;
    private RoomInformation room = null;

    private int[] tris = new int[0];
    private List<NavMeshTriangle> burningTris = new List<NavMeshTriangle>();
    private List<NavMeshTriangle> availableTris = new List<NavMeshTriangle>();
    private List<NavMeshTriangle> availableDoorTriangles = new List<NavMeshTriangle>();
    private Dictionary<int, NavMeshEntrance> burningTriAvailableDoorLink = new Dictionary<int, NavMeshEntrance>();

    private bool isOnFire = false;

    [SerializeField] private float fireLifetime = 5;
    private float fireLifeTimer = 0;
    private int removedTriangles = 0;

    public bool IsOnFire => isOnFire;

    [SerializeField] private int numberOfSteps = 100;
    private int fireStepWaitCounter = 0;
    [SerializeField] private float afterBurnTime = 10;
    private float afterBurnTimer = 0;

    public bool CanBurn => afterBurnTimer <= 0;

    private void Awake()
    {
        mesh = new Mesh();
        afterBurnTimer = 0;
    }

    public void StartFire(RoomInformation room, int triangleID)
    {
        if (isOnFire)
        {
            return;
        }
        mesh = new Mesh();
        isOnFire = true;
        particleSystem.Play();
        smokeParticleSystem.Play();

        fireLifeTimer = fireLifetime;

        this.room = room;
        Vertex[] vertices = room.NavMeshGenerator.Vertexes;
        Vector3[] verts = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            verts[i] = vertices[i].LocalPosition;
        }
        NavMeshTriangle[] triangles = room.NavMeshGenerator.Triangles;
        tris = new int[triangles.Length * 3];

        AddTriangle(tris, triangles[triangleID], burningTris, availableTris);
        mesh.vertices = verts;
        mesh.triangles = tris;
        filler.mesh = mesh;
        var sh = particleSystem.shape;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = mesh;
        sh = smokeParticleSystem.shape;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = mesh;
    }

    private void Update()
    {
        if(afterBurnTimer > 0)
        {
            afterBurnTimer -= Time.deltaTime;
        }

        if (!isOnFire)
        {
            return;
        }
        if (fireLifeTimer > 0)
        {
            fireLifeTimer -= Time.deltaTime;
            StepFireForwards();
        }
        else if(burningTris.Count > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                tris[removedTriangles * 3 + i] = 0;
            }
            removedTriangles++;
            availableDoorTriangles.Remove(burningTris[0]);
            burningTris.RemoveAt(0);
            if(burningTris.Count == 0)
            {
                particleSystem.Stop();
                smokeParticleSystem.Stop();
                mesh.triangles = tris;
                var sh = particleSystem.shape;
                sh.shapeType = ParticleSystemShapeType.Sphere;
                sh = smokeParticleSystem.shape;
                sh.shapeType = ParticleSystemShapeType.Sphere;
                burningTris.Clear();
                removedTriangles = 0;
                isOnFire = false;
                afterBurnTimer = afterBurnTime;
            }
            else
            {
                mesh.triangles = tris;
                var sh = particleSystem.shape;
                sh.mesh = mesh;
                sh = smokeParticleSystem.shape;
                sh.mesh = mesh;
            }
        }
        if (availableDoorTriangles.Count > 0)
        {
            for (int i = 0; i < availableDoorTriangles.Count; i++)
            {
                for (int j = 0; j < availableDoorTriangles[i].connectedEntranceIDs.Length; j++)
                {
                    NavMeshEntrance selectedEntrance = room.GetEntrance(availableDoorTriangles[i].connectedEntranceIDs[j]);
                    if (selectedEntrance.connectedEntrance != null && selectedEntrance.IsPassable)
                    {
                        RoomInformation nextRoom = room.GetConnectedRoomFromEntranceWithID(selectedEntrance.ID);
                        if (nextRoom)
                        {
                            nextRoom.StartFire(selectedEntrance.connectedEntrance.connectedTriangle.ID);
                        }
                    }
                }
            }
        }
    }

    private void StepFireForwards()
    {
        if (availableTris.Count > 0 && fireStepWaitCounter-- <= 0)
        {
            fireStepWaitCounter = numberOfSteps;
            AddTriangle(tris, availableTris[0], burningTris, availableTris);

            availableTris.RemoveAt(0);

            mesh.triangles = tris;
            var sh = particleSystem.shape;
            sh.mesh = mesh;
            sh = smokeParticleSystem.shape;
            sh.mesh = mesh;
        }
    }

    private void AddTriangle(int[] tris, NavMeshTriangle triangle, List<NavMeshTriangle> burningTris, List<NavMeshTriangle> availableTris)
    {
        int index = burningTris.Count;

        burningTris.Add(triangle);
        foreach (int adjTriangleID in triangle.adjacentTriangles)
        {
            NavMeshTriangle adjTriangle = room.NavMeshGenerator.FindTriangleWithID(adjTriangleID);
            if(!burningTris.Contains(adjTriangle) && !availableTris.Contains(adjTriangle))
            {
                availableTris.Add(adjTriangle);
            }
        }

        if (triangle.isConnectedToEntrance)
        {
            availableDoorTriangles.Add(triangle);
        }

        for (int j = 0; j < room.NavMeshGenerator.Vertexes.Length; j++)
        {
            if (room.NavMeshGenerator.Vertexes[j].Equals(triangle.vertex1))
            {
                tris[index * 3] = j;
                break;
            }
        }
        for (int j = 0; j < room.NavMeshGenerator.Vertexes.Length; j++)
        {
            if (room.NavMeshGenerator.Vertexes[j].Equals(triangle.vertex2))
            {
                tris[index * 3 + 1] = j;
                break;
            }
        }
        for (int j = 0; j < room.NavMeshGenerator.Vertexes.Length; j++)
        {
            if (room.NavMeshGenerator.Vertexes[j].Equals(triangle.vertex3))
            {
                tris[index * 3 + 2] = j;
                break;
            }
        }
    }
}
