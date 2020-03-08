using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pieter.NavMesh
{
    public class Vertex : MonoBehaviour
    {
        public int ID = -1;
        public Vector3 Position => this.transform.position;
        public List<Vertex> adjacent;


        public override bool Equals(object other)
        {
            Vertex vert = (Vertex)other;
            return vert.Position == Position;
        }

        public override string ToString()
        {
            return ID + " : " +Position.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
