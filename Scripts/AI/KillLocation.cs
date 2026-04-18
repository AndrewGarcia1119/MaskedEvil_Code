using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    /*
    
    IMPORTANT: The kill locations should not overlap.
    
    */
    [RequireComponent(typeof(Collider))]
    //[RequireComponent(typeof(Rigidbody))]
    public class KillLocation : MonoBehaviour
    {
        public static event Action<AIController> onCharacterEnteredKillzone;
        public static event Action<AIController> onCharacterLeftKillzone;

        private void Start()
        {
            gameObject.layer = 2;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent<AIController>(out AIController possibleKillable);
            if (!possibleKillable) return;
            onCharacterEnteredKillzone?.Invoke(possibleKillable);
        }

        private void OnTriggerExit(Collider other)
        {
            other.gameObject.TryGetComponent<AIController>(out AIController possibleKillable);
            if (!possibleKillable) return;
            onCharacterLeftKillzone?.Invoke(possibleKillable);
        }
    }

}