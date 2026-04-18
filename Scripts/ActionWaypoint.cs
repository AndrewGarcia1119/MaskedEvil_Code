using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class ActionWaypoint : Waypoint
{
    public AIController.Actions associatedAction;

    private bool hasSetAction = false;
    public ActionWaypoint(Vector3 pos, AIController activeWaypointUser, AIController.Actions associatedAction) : base(pos, activeWaypointUser)
    {
        this.associatedAction = associatedAction;
    }

    public void SetAIAction()
    {
        if (!hasSetAction)
        {
            activeWaypointUser.SetAction(associatedAction);
            hasSetAction = true;
        }
    }
}
