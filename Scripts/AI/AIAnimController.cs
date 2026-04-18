using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class AIAnimController : MonoBehaviour
    {
        [SerializeField]
        Animator aiAnimator = null;

        [SerializeField]
        NavMeshAgent agent;

        [SerializeField]
        Collider outerCollider;

        [SerializeField]
        AIController ai;

        [SerializeField]
        private RuntimeAnimatorController[] dancingAnimatorControllers = null;

        private bool isRagdoll = false;
        private RuntimeAnimatorController defaultAnimController;
        private RuntimeAnimatorController randomlySetAnim;
        private RuntimeAnimatorController dancingAnimatorController;

        private void Start()
        {
            defaultAnimController = aiAnimator.runtimeAnimatorController;
            dancingAnimatorController = dancingAnimatorControllers[Random.Range(0, dancingAnimatorControllers.Length)];
        }
        private void Update()
        {
            if (!aiAnimator || !agent) return;
            if (ai.HasCurrentAction(out AIController.Actions action))
            {
                if (action == AIController.Actions.DANCING)
                {
                    SetAnimController(dancingAnimatorController);
                }
            }
            else
            {
                SetAnimController(defaultAnimController);
                aiAnimator.SetFloat("Blend", agent.velocity.magnitude);
            }
            if (ai.isDead && !isRagdoll)
            {
                Ragdoll();
            }
        }

        public void Ragdoll()
        {
            isRagdoll = true;
            outerCollider.enabled = false;
            aiAnimator.enabled = false;
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
        }

        private void SetAnimController(RuntimeAnimatorController newController)
        {
            if (aiAnimator.runtimeAnimatorController != newController)
            {
                aiAnimator.runtimeAnimatorController = newController;
            }
        }
    }

}