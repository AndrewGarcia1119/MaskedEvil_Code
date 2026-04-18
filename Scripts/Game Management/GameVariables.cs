using System.Collections;
using System.Collections.Generic;
using AI;
using Player;
using UnityEngine;

namespace GameManagement
{
    public class GameVariables : MonoBehaviour
    {
        public AIController designatedSurvivor;
        //These variables below are internal because only the GameManager should access these. If they need to be public, please let me know, I don't want multiple classes accessing these at the same time
        internal List<AIController> livingBystanders = new List<AIController>();
        internal HashSet<AIController> assassins = new HashSet<AIController>();
        internal HashSet<AIController> deadCharacters = new HashSet<AIController>();
        internal List<AIController> markedCharacters = new List<AIController>();
        internal HashSet<Sniper> snipers = new HashSet<Sniper>();

        private void Start()
        {
            foreach (AIController aiController in FindObjectsOfType<AIController>())
            {
                if (aiController == designatedSurvivor) continue;

                if (!aiController.IsAssassin())
                {
                    if (aiController != designatedSurvivor)
                        livingBystanders.Add(aiController);
                }
                else
                {
                    assassins.Add(aiController);
                }
            }
            foreach (Sniper sniper in FindObjectsOfType<Sniper>(true)) //This true bool allows inactive objects to be found as well
            {
                snipers.Add(sniper);
            }
        }

        public bool IsInLivingBystanders(AIController character)
        {
            return livingBystanders.Contains(character);
        }

        public int getMarkedCount()
        {
            return markedCharacters.Count;
        }
    }
}