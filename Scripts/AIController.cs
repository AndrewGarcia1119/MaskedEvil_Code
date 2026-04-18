using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using GameManagement;
/*
TODO: Make sure assassin can randomly choose a killable character. Also make sure to go back to standard waypoint generation if character dies to another assassin, or the character leaves kill location.

AFTER ALPHA SUBMISSION:
-- Make sure assassin only targets players if near the kill location, this will help reduce random suspicious movement if the assassin is further from the kill zone.
-- Make sure bystanders stay in kill locations longer, this gives assassins a higher chance of successfully killing a target
*/






namespace AI
{
    public class AIController : MonoBehaviour, IShootable, IActor
    {
        [Header("Behavior")]
        [SerializeField]
        private float generationCooldownMin = 4f, generationCooldownMax = 6f;
        [SerializeField]
        private bool dances = true;

        [SerializeField]
        private float dancingTime = 10f;

        [Header("On Death")]
        //When the character dies, it will be pushed upwards, this is the intensity of that force
        [SerializeField]
        float impulseMultiplier = 15f;
        //When a characer dies, it will rotate as well, this determines the intensity of the rotation
        [SerializeField]
        float rotationMultiplier = 25f;

        [Header("Bias")]
        [Tooltip("You specify the weight of specific generators, the weight for any generator not mention is 1. This is meant to only use non-negative values")]
        [SerializeField]
        private GeneratorWeight[] weightedGenerators = null;

        [Header("Assassination")]
        [SerializeField]
        protected bool isAssassin = false;
        [SerializeField]
        protected bool assassinate = false;
        [SerializeField]
        protected float assassinationDistance = 2.5f;
        [SerializeField]
        protected int assassinationFrequency = 3;
        [SerializeField]
        protected int minePlacementFrequency = 3;
        [SerializeField]
        private float mineDropDistance = 2f;
        [SerializeField]
        private GameObject assassinWeapon;
        [SerializeField]
        private float weaponAppearanceDistance = 8f;

        public GameObject mine;

        [Header("Marker")]
        [SerializeField]
        private GameObject marker;

        [HideInInspector]
        public bool isDead = false;
        [HideInInspector]
        public Dictionary<WaypointGenerator, float> generatorWeights = new Dictionary<WaypointGenerator, float>();

        //references
        protected NavMeshAgent agent;
        protected WaypointManager pointManager;
        protected GameVariables gv;
        protected KillLocationManager klm;

        //states
        protected Waypoint currentWaypoint = null;
        protected float timer;
        protected bool overrideAssassin = false;
        protected float generationCooldown;
        protected AIController target;
        private IAction currentAction = null;
        private readonly DanceAction danceAction = new();
        private readonly MinePlacementAction minePlacementAction = new();
        private bool diedWhileMarked = false;

        //events
        public static event Action<AIController, bool> onKilled;
        public static event Action<AIController, bool> onMarkToggled;

        private void Awake()
        {
            generationCooldown = UnityEngine.Random.Range(generationCooldownMin, generationCooldownMax);
        }

        // Start is called before the first frame update
        void Start()
        {
            gv = FindObjectOfType<GameVariables>();
            if (isAssassin)
            {

                klm = FindObjectOfType<KillLocationManager>();
            }
            if (weightedGenerators != null && weightedGenerators.Length > 0)
            {
                AddGeneratorWeightsToDictionary();
            }

            if (marker)
            {
                marker.SetActive(false);
            }
            agent = GetComponent<NavMeshAgent>();

            pointManager = FindObjectOfType<WaypointManager>();

            currentWaypoint = pointManager.GenerateWaypoint(this);
            timer = 5f;
            ToggleAssassinWeapon(false);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (isDead) return;
            if (!overrideAssassin)
            {
                if (currentAction != null)
                {
                    currentAction.Run(this);
                }
                else if (assassinate)
                {
                    if (!klm.killableCharacters.Contains(target)) assassinate = false;
                    currentWaypoint = new Waypoint(target.transform.position, this);
                    float sqrDist = Vector3.SqrMagnitude(transform.position - target.transform.position);
                    ToggleAssassinWeapon(sqrDist < weaponAppearanceDistance * weaponAppearanceDistance);
                    if (sqrDist < assassinationDistance * assassinationDistance)
                    {
                        target.OnHit(false, gameObject.transform);
                        assassinate = false;
                    }
                }
                else if (timer >= generationCooldown)
                {

                    if (isAssassin && !assassinate)
                    {
                        ToggleAssassinWeapon(false);
                        int random = UnityEngine.Random.Range(0, assassinationFrequency);
                        if (random == 0)
                        {
                            GetNewTarget();
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
                if (currentWaypoint is ActionWaypoint waypoint)
                {
                    if (agent.velocity == Vector3.zero && Vector3.SqrMagnitude(transform.position - currentWaypoint.pos) < 2)

                    //if (agent.isStopped)
                    {
                        waypoint.SetAIAction();
                    }
                }

            }
            else
            {
                if (gv.designatedSurvivor.isDead) overrideAssassin = false;
                currentAction = null;
                currentWaypoint = new Waypoint(gv.designatedSurvivor.transform.position, this);
                float sqrDist = Vector3.SqrMagnitude(transform.position - gv.designatedSurvivor.transform.position);
                ToggleAssassinWeapon(sqrDist < weaponAppearanceDistance * weaponAppearanceDistance);
                if (sqrDist < assassinationDistance * assassinationDistance)
                {
                    gv.designatedSurvivor.OnHit(false, gameObject.transform);
                    overrideAssassin = false;
                }
            }

            agent.SetDestination(currentWaypoint.pos);
            timer += Time.deltaTime;
        }

        private void ToggleAssassinWeapon(bool v)
        {
            if (!assassinWeapon) return;
            if (assassinWeapon.activeSelf == v) return;
            assassinWeapon.SetActive(v);
        }

        private void Die(bool killedByPlayer)
        {
            isDead = true;
            onKilled?.Invoke(this, killedByPlayer);
            agent.enabled = false;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            if (IsMarked())
            {
                diedWhileMarked = true;
            }
            ToggleMark(false);
            rb.AddForce(transform.up * impulseMultiplier + transform.forward * 3f, ForceMode.Impulse);
            rb.AddTorque(transform.forward * rotationMultiplier);
        }

        public void OnHit(bool hitByPlayer, Transform transform)
        {
            if (isDead)
            {
                return;
            }
            Die(hitByPlayer);
        }

        public bool IsMarked()
        {
            return marker.activeInHierarchy;
        }

        public void ToggleMark(bool mark)
        {
            if (isDead) return;
            if (this == gv.designatedSurvivor)
            {
                return;
            }
            if (!marker)
            {
                Debug.LogWarning("Character needs a marker for marking to work");
            }
            if (IsMarked() != mark)
            {
                marker.SetActive(mark);
                onMarkToggled?.Invoke(this, mark);
            }
        }

        public void OverrideAssassinKillDS()
        {
            overrideAssassin = true;
            assassinate = false;
        }

        public void GetNewTarget()
        {
            //if (gv.livingBystanders.Count == 0) return;
            //target = gv.livingBystanders[UnityEngine.Random.Range(0, gv.livingBystanders.Count)];
            if (klm.killableCharacters.Count == 0) return;
            target = klm.killableCharacters[UnityEngine.Random.Range(0, klm.killableCharacters.Count)];
            assassinate = true;
        }

        public void SetAction(Actions action)
        {
            if (action == Actions.NO_ACTION)
            {
                currentAction = null;
            }
            else if (action == Actions.DANCING)
            {
                currentAction = danceAction;
            }
            else if (action == Actions.PLACE_MINE)
            {
                currentAction = minePlacementAction;
            }
            else
            {
                currentAction = null;
            }
        }

        public bool HasCurrentAction(out Actions action)
        {
            if (currentAction == null)
            {
                action = Actions.NO_ACTION;
                return false;
            }
            else
            {
                action = currentAction.GetActionType();
                return true;
            }
        }

        public bool IsAssassin()
        {
            return isAssassin;
        }

        public bool DiedWhileMarked()
        {
            return diedWhileMarked;
        }

        public void SpawnMine()
        {
            Instantiate(mine, new Vector3(transform.position.x + mineDropDistance * -transform.forward.x, transform.position.y + 1, transform.position.z + mineDropDistance * -transform.forward.z), transform.rotation);
        }

        [Serializable]
        private struct GeneratorWeight
        {
            public WaypointGenerator generator;
            public float weightMultiplier;
        }

        private void AddGeneratorWeightsToDictionary()
        {
            foreach (GeneratorWeight gw in weightedGenerators)
            {
                generatorWeights.Add(gw.generator, gw.weightMultiplier);
            }
        }

        private interface IAction
        {
            /// <summary>
            /// When active, this function runs every single frame, if you want to make it an instant action, you must set the current action to null, or set it to another action for a transition
            /// </summary>
            void Run(AIController outerClass);
            Actions GetActionType();
        }

        private class DanceAction : IAction
        {
            //private AIController outerController = null;
            private float danceTimer = 0;

            public Actions GetActionType()
            {
                return Actions.DANCING;
            }

            //The code for setting the dance animation is handled by AIAnimController, so there isn't much to do here except for stopping the action at the correct time.
            public void Run(AIController outerClass)
            {
                if (!outerClass.dances)
                {
                    outerClass.SetAction(Actions.NO_ACTION);
                    return;
                }
                danceTimer += Time.deltaTime;
                if (danceTimer >= outerClass.dancingTime)
                {
                    danceTimer = 0;
                    outerClass.SetAction(Actions.NO_ACTION);
                }

            }
        }

        private class MinePlacementAction : IAction
        {

            //  private AIController outerController = null;
            public Actions GetActionType()
            {
                return Actions.PLACE_MINE;
            }

            public void Run(AIController outerClass)
            {
                if (!outerClass.isAssassin)
                {
                    outerClass.currentAction = null;
                    return;
                }
                int random = UnityEngine.Random.Range(0, outerClass.minePlacementFrequency);
                if (random == 0)
                {
                    outerClass.SpawnMine();
                }

                outerClass.currentAction = null;
            }
        }

        [Serializable]
        public enum Actions
        {
            NO_ACTION, DANCING, PLACE_MINE
        }
    }
}
