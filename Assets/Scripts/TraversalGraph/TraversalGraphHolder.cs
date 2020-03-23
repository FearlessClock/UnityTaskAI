using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pieter.GraphTraversal
{
    public class TraversalGraphHolder : MonoBehaviour
    {
        [SerializeField] private TraversalLine[] traversalLines = null;
        private TraversalGenerator[] generators = new TraversalGenerator[0];

        public TraversalLine[] TraversalLines { get { return traversalLines; } }


        private Action OnValueUpdated;
        public void AddListener(Action call)
        {
            OnValueUpdated += call;
        }
        public void RemoveListener(Action call)
        {
            OnValueUpdated -= call;
        }
        public void Notify()
        {
            OnValueUpdated?.Invoke();
        }

        // Add a new traversal line the the holder, also add the generator to the holder
        public void AddTraversalLines(TraversalGenerator generators)
        {
            List<TraversalLine> collectedLines = new List<TraversalLine>();
            collectedLines.AddRange(traversalLines);
            collectedLines.AddRange(generators.TraversalLines);
            traversalLines = collectedLines.ToArray();

            List<TraversalGenerator> collectedGenerators = new List<TraversalGenerator>();
            collectedGenerators.AddRange(this.generators);
            collectedGenerators.Add(generators);
            this.generators = collectedGenerators.ToArray();

            OnValueUpdated?.Invoke();
        }
        
        public TraversalLine GetRandomLine()
        {
            if (traversalLines.Length == 0)
            {
                return null;
            }
            int index = Random.Range(0, traversalLines.Length);

            return traversalLines[index];
        }

        public TraversalLine GetMiddleLineForCurrentGenerator(TraversalGenerator generator)
        {
            for (int i = 0; i < generator.TraversalLines.Length; i++)
            {
                if (generator.TraversalLines[i].vertex.Equals(generator.MiddleVertex))
                {
                    return generator.TraversalLines[i];
                }
            }
            return null;
        }

        public TraversalGenerator GetClosestGenerator(Vector3 pos)
        {
            if (generators == null || generators.Length == 0)
            {
                return null;
            }
            int index = 0;
            float distance = DistanceToGenerator(pos, generators[0]);
            for (int i = 0; i < generators.Length; i++)
            {
                if (distance > DistanceToGenerator(pos, generators[i]))
                {
                    index = i;
                    distance = DistanceToGenerator(pos, generators[i]);
                }
            }

            return generators[index];
        }

        private float DistanceToGenerator(Vector3 pos, TraversalGenerator gen)
        {
            return Mathf.Pow(pos.x - gen.MiddleVertex.Position.x, 2) +
                   Mathf.Pow(pos.y - gen.MiddleVertex.Position.y, 2) +
                   Mathf.Pow(pos.z - gen.MiddleVertex.Position.z, 2);
        }

        public void AddTraversalGraph(TraversalGenerator traversalGenerator)
        {
            AddTraversalLines(traversalGenerator);
        }

        public void ResetData()
        {
            traversalLines = new TraversalLine[0];
        }
    }
}
