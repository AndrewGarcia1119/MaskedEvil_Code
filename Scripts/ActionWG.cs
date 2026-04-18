using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class ActionWG : WaypointGenerator
    {
        [SerializeField] AIController.Actions action;

        public override Waypoint GenerateWaypoint(AIController aiController)
        {
            Vector3 newPoint = new Vector3(Random.Range(transform.position.x - x / 2, transform.position.x + x / 2),
                    Random.Range(transform.position.y - y / 2, transform.position.y + y / 2),
                    Random.Range(transform.position.z - z / 2, transform.position.z + z / 2));
            Vector3 adjustedPos;
            if (NavMesh.SamplePosition(newPoint, out NavMeshHit hit, 500, NavMesh.AllAreas))
            {
                adjustedPos = hit.position;
            }
            else
            {
                return GenerateWaypoint(aiController);
            }

            return new ActionWaypoint(adjustedPos, aiController, action);
        }
    }

}