using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class testAIController : AIController
{

    private const int DROP_DIST = 2;

    protected override void Update()
    {

        if (isDead) return;
        if (!overrideAssassin)
        {
            if (assassinate)
            {
                if (!klm.killableCharacters.Contains(target)) assassinate = false;
                currentWaypoint = new Waypoint(target.transform.position, this);
                if (Vector3.SqrMagnitude(transform.position - target.transform.position) < assassinationDistance * assassinationDistance)
                {
                    target.OnHit(false, gameObject.transform);
                    assassinate = false;
                }
            }
            else if (timer >= generationCooldown)
            {
                if (isAssassin && !assassinate)
                {
                    int random = UnityEngine.Random.Range(0, assassinationFrequency);
                    if (random == 0)
                    {
                        GetNewTarget();
                    }
                    else if (random == 1)
                    {
                        SpawnMine();
                    }
                }
                if (generatorWeights.Count == 0)
                {
                    pointManager.RemoveWaypoint(currentWaypoint);
                    currentWaypoint = pointManager.GenerateWaypoint(this);
                    timer = 0f;
                }
                else
                {
                    pointManager.RemoveWaypoint(currentWaypoint);
                    currentWaypoint = pointManager.GenerateWaypointWeighted(this);
                    timer = 0f;
                }
            }
        }
        else
        {
            if (gv.designatedSurvivor.isDead) overrideAssassin = false;
            currentWaypoint = new Waypoint(gv.designatedSurvivor.transform.position, this);
            if (Vector3.SqrMagnitude(transform.position - gv.designatedSurvivor.transform.position) < assassinationDistance * assassinationDistance)
            {
                gv.designatedSurvivor.OnHit(false, gameObject.transform);
                overrideAssassin = false;
            }
        }

        agent.SetDestination(currentWaypoint.pos);
        timer += Time.deltaTime;

    }

}
