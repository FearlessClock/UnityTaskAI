using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pieter.NavMesh;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NavMeshNavigationTest
    {
        GameObject obj = null;
        NavMeshGenerator meshGenerator = null;
        NavMeshHolder navMeshHolder;

        [SetUp]
        public void Setup()
        {
            obj = new GameObject();
            navMeshHolder = obj.AddComponent<NavMeshHolder>();
            meshGenerator = obj.AddComponent<NavMeshGenerator>();
        }

        [Test]
        public void NavMeshNavigationTest_EmptyHolder()
        {
            AStarNavMeshNavigation navMesh = new AStarNavMeshNavigation(navMeshHolder);

            List<NavMeshMovementLine> path = navMesh.GetPathFromTo(Vector3.zero, Vector3.one*5);
            Assert.AreEqual(0, path.Count);
        }

        private Vertex CreateVertex(Vector3 pos, int id)
        {
            Vertex vert = new GameObject().AddComponent<Vertex>();
            vert.transform.position = pos;
            vert.transform.parent = meshGenerator.transform;
            vert.ID = id;
            return vert;
        }

        [Test]
        public void NavMeshNavigationTest_SmallHolder()
        {
            Vertex vert1 = CreateVertex(new Vector3(0, 0), 1);
            Vertex vert2 = CreateVertex(new Vector3(0, 10), 2);
            Vertex vert3 = CreateVertex(new Vector3(10, 0), 3);

            meshGenerator.CreateTriangle(vert1, vert2, vert3);
            navMeshHolder.AddNavMesh(meshGenerator);
            AStarNavMeshNavigation navMesh = new AStarNavMeshNavigation(navMeshHolder);

            List<NavMeshMovementLine> path = navMesh.GetPathFromTo(new Vector3(1, 1), new Vector3(4, 4), true, true);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(new Vector3(1, 1), path[0].point);
            Assert.AreEqual(new Vector3(4, 4), path[1].point);
        }

        [Test]
        public void NavMeshNavigationTest_AdjacentTriangle()
        {
            Vertex vert11 = CreateVertex(new Vector3(0, 0), 1);
            Vertex vert12 = CreateVertex(new Vector3(0, 10), 2);
            Vertex vert13 = CreateVertex(new Vector3(10, 0), 3);
            Vertex vert22 = CreateVertex(new Vector3(0, 10), 4);
            Vertex vert21 = CreateVertex(new Vector3(10, 10), 5);
            Vertex vert23 = CreateVertex(new Vector3(10, 0), 6);

            meshGenerator.CreateTriangle(vert11, vert12, vert13);
            meshGenerator.CreateTriangle(vert21, vert22, vert23);
            navMeshHolder.AddNavMesh(meshGenerator);
            AStarNavMeshNavigation navMesh = new AStarNavMeshNavigation(navMeshHolder);

            List<NavMeshMovementLine> path = navMesh.GetPathFromTo(new Vector3(1, 1), new Vector3(7, 7), true, true);
            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log(path[i].point);
            }
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(new Vector3(1, 1), path[0].point);
            Assert.AreEqual(new Vector3(7, 7), path[1].point);
        }

        [Test]
        public void NavMeshNavigationTest_FutherAwayTriangle()
        {
            Vertex vert1 = CreateVertex(new Vector3(0, 0, 0), 0);
            Vertex vert2 = CreateVertex(new Vector3(0, 0, 10), 1);
            Vertex vert3 = CreateVertex(new Vector3(10, 0, 0), 2);
            Vertex vert4 = CreateVertex(new Vector3(10, 0, 10), 3);
            Vertex vert5 = CreateVertex(new Vector3(20, 0, 10), 4);
            Vertex vert6 = CreateVertex(new Vector3(10, 0, 20), 5);
            CreateAndAddTriangle(vert1, vert2, vert3);
            CreateAndAddTriangle(vert2, vert4, vert3);
            CreateAndAddTriangle(vert4, vert5, vert3);
            CreateAndAddTriangle(vert4, vert6, vert5);

            meshGenerator.UpdateAdjacentVertexesAndTriangleID();
            meshGenerator.UpdateAdjacentTrianglesWithoutThreads();

            navMeshHolder.AddNavMesh(meshGenerator);
            AStarNavMeshNavigation navMesh = new AStarNavMeshNavigation(navMeshHolder);

            List<NavMeshMovementLine> path = navMesh.GetPathFromTo(new Vector3(0.5f, 0, 9), new Vector3(10.5f, 0, 19), true, true);
            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log(path[i].point);
            }
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(new Vector3(0.5f, 0, 9), path[0].point);
            Assert.AreEqual(new Vector3(10.5f, 0, 19), path[2].point);
        }

        private void CreateAndAddTriangle(Vertex posA, Vertex posB, Vertex posC)
        {
            meshGenerator.CreateTriangle(posA, posB, posC);
        }

        [Test]
        public void NavMeshNavigationTest_StartingPointOutsideMesh()
        {
            Vertex vert11 = CreateVertex(new Vector3(0, 0), 0);
            Vertex vert12 = CreateVertex(new Vector3(0, 10), 1);
            Vertex vert13 = CreateVertex(new Vector3(10, 0), 2);
            Vertex vert22 = CreateVertex(new Vector3(0, 10), 3);
            Vertex vert21 = CreateVertex(new Vector3(10, 10), 4);
            Vertex vert23 = CreateVertex(new Vector3(10, 0), 5);

            meshGenerator.CreateTriangle(vert11, vert12, vert13);
            meshGenerator.CreateTriangle(vert21, vert22, vert23);
            navMeshHolder.AddNavMesh(meshGenerator);
            AStarNavMeshNavigation navMesh = new AStarNavMeshNavigation(navMeshHolder);

            List<NavMeshMovementLine> path = navMesh.GetPathFromTo(new Vector3(-4, 1), new Vector3(7, 7), true, true);
            Assert.AreEqual(0, path.Count);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(obj);
        }
    }
}
