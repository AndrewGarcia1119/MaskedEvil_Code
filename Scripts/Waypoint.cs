using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Waypoint
    {
        public Vector3 pos;
        public AIController activeWaypointUser;

        public Waypoint(Vector3 pos, AIController activeWaypointUser)
        {
            this.pos = pos;
            this.activeWaypointUser = activeWaypointUser;
        }
    }

}