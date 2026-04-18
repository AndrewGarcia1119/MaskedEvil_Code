using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class SniperAnimEvents : MonoBehaviour
    {
        [SerializeField]
        private Sniper sniper;

        private void OnEnable()
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

        public void Aim()
        {
            if (!sniper)
            {
                sniper = GetComponentInParent<Sniper>();
            }
            if (!sniper)
            {
                Debug.LogWarning("ATTACH Object with Sniper component to the object with SniperAnimEvents component");
            }
            sniper.Aim();
            GetComponent<MeshRenderer>().enabled = false;
        }
        public void EndAim()
        {
            if (!sniper)
            {
                sniper = GetComponentInParent<Sniper>();
            }
            if (!sniper)
            {
                Debug.LogWarning("ATTACH Object with Sniper component to the object with SniperAnimEvents component");
            }
            sniper.EndAim();
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}