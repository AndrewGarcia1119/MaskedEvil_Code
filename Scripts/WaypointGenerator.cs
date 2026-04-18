using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class WaypointGenerator : MonoBehaviour
    {
        [SerializeField]
        protected float x = 1f, y = 1f, z = 1f;

        public virtual Waypoint GenerateWaypoint(AIController aiController)
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

            return new Waypoint(adjustedPos, aiController);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(x, y, z));
        }
    }
}