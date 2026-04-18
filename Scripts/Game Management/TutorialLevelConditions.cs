using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement.System
{
    public class TutorialLevelConditions : AbstractGameConditions
    {
        public override bool FirstConditionCompleted()
        {
            return true;
        }

        public override bool SecondConditonCompleted()
        {
            return true;
        }

        public override bool ThirdConditionCompleted()
        {
            return true;
        }

    }
}