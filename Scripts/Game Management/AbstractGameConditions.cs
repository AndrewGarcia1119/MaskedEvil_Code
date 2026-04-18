using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement.System
{
    public abstract class AbstractGameConditions : MonoBehaviour
    {

        protected GameManager gm;
        // Start is called before the first frame update
        private void Start()
        {
            gm = FindObjectOfType<GameManager>();
        }

        public abstract bool FirstConditionCompleted();
        public abstract bool SecondConditonCompleted();
        public abstract bool ThirdConditionCompleted();
    }

}