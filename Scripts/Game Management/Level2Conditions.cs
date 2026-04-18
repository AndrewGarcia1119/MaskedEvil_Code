using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement.System
{
    public class Level2Conditions : AbstractGameConditions
    {
        [SerializeField]
        KillOnTouch chandelier;


        private bool assassinKilledByChandelier = false;
        private void OnEnable()
        {
            chandelier.onKillAssassin += AssassinKilledByChandelier;
        }

        private void OnDisable()
        {
            chandelier.onKillAssassin -= AssassinKilledByChandelier;
        }
        public override bool FirstConditionCompleted()
        {
            return assassinKilledByChandelier;
        }

        public override bool SecondConditonCompleted()
        {
            return gm.GetCurrentTimeLeft() > (gm.GetTotalTime() / 2);
        }

        public override bool ThirdConditionCompleted()
        {
            return gm.GetAmmoCount() >= gm.GetStartingAmmoCount() - 4;
        }

        private void AssassinKilledByChandelier()
        {
            assassinKilledByChandelier = true;
        }
    }
}