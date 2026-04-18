using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class WaypointManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("determines how close any waypoint can be to another")]
        private float wayPointTolerance = 1.0f;

        [SerializeField]
        private float defaultGeneratorWeight = 1;

        private List<WaypointGenerator> waypointGenerators = new List<WaypointGenerator>();

        private HashSet<Waypoint> waypoints = new HashSet<Waypoint>();

        bool waypointsGenerated = false;


        public Waypoint GenerateWaypoint(AIController aiController)
        {
            InitializeGenerators();
            int random = Random.Range(0, waypointGenerators.Count);
            Waypoint waypoint = waypointGenerators[random].GenerateWaypoint(aiController);
            if (TooCloseToAnotherPoint(waypoint)) return GenerateWaypoint(aiController);
            waypoints.Add(waypoint);
            return waypoint;
        }


        public bool RemoveWaypoint(Waypoint point)
        {
            return waypoints.Remove(point);
        }
        public Waypoint GenerateWaypointWeighted(AIController aiController)
        {
            InitializeGenerators();
            if (aiController == null || aiController.generatorWeights == null || aiController.generatorWeights.Count == 0) return GenerateWaypoint(aiController);
            float total = 0f;
            foreach (WaypointGenerator wg in waypointGenerators)
            {
                total += aiController.generatorWeights.ContainsKey(wg) ? (aiController.generatorWeights[wg] * defaultGeneratorWeight) : defaultGeneratorWeight;
            }
            float randomVal = Random.Range(0f, total);
            foreach (WaypointGenerator wg in waypointGenerators)
            {
                if (aiController.generatorWeights.ContainsKey(wg))
                {
                    float genWeight = aiController.generatorWeights[wg] * defaultGeneratorWeight;
                    if (randomVal < genWeight && genWeight != 0)
                    {
                        Waypoint waypoint = wg.GenerateWaypoint(aiController);
                        if (TooCloseToAnotherPoint(waypoint)) return GenerateWaypointWeighted(aiController);
                        waypoints.Add(waypoint);
                        return waypoint;
                    }
                    randomVal -= genWeight;
                }
                else
                {
                    if (randomVal < defaultGeneratorWeight)
                    {
                        Waypoint waypoint = wg.GenerateWaypoint(aiController);
                        if (TooCloseToAnotherPoint(waypoint)) return GenerateWaypointWeighted(aiController);
                        waypoints.Add(waypoint);
                        return waypoint;
                    }
                    randomVal -= defaultGeneratorWeight;
                }

            }
            return GenerateWaypointWeighted(aiController);
        }

        private bool TooCloseToAnotherPoint(Waypoint waypoint)
        {
            foreach (Waypoint point in waypoints)
            {
                if (Vector3.SqrMagnitude(waypoint.pos - point.pos) < wayPointTolerance * wayPointTolerance)
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeGenerators()
        {
            if (!waypointsGenerated)
            {
                foreach (WaypointGenerator gen in FindObjectsOfType<WaypointGenerator>())
                {
                    if (!gen.gameObject.activeSelf || !gen.gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    waypointGenerators.Add(gen);
                }
                waypointsGenerated = true;
            }
        }
    }

}