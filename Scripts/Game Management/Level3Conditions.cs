using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

namespace GameManagement.System
{
    public class Level3Conditions : AbstractGameConditions
    {

        [SerializeField]
        AIController assassin;

        public override bool FirstConditionCompleted()
        {
            return gm.GetCurrentScore() > 70;
        }

        public override bool SecondConditonCompleted()
        {
            return assassin.DiedWhileMarked();
        }

        public override bool ThirdConditionCompleted()
        {
            return gm.GetAmmoCount() >= gm.GetStartingAmmoCount() - 1;
        }

    }
}