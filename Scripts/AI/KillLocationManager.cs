using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
using Unity.VisualScripting;

namespace AI
{
    //
    // TODO: Make sure killableCharacters removes dead characters
    //
    public class KillLocationManager : MonoBehaviour
    {
        internal List<AIController> killableCharacters = new List<AIController>();

        private HashSet<KillLocation> killLocations = new HashSet<KillLocation>();

        private GameVariables gv;
        private bool killLocationsAdded;

        private void OnEnable()
        {
            KillLocation.onCharacterEnteredKillzone += HandleCharacterEnteredKillzone;
            KillLocation.onCharacterLeftKillzone += HandleCharacterLeftKillzone;
            AIController.onKilled += HandleCharacterKilled;
        }

        private void Start()
        {
            InitializeGameVariables();
        }

        private void OnDisable()
        {
            KillLocation.onCharacterEnteredKillzone -= HandleCharacterEnteredKillzone;
            KillLocation.onCharacterLeftKillzone -= HandleCharacterLeftKillzone;
            AIController.onKilled += HandleCharacterKilled;
        }

        private void HandleCharacterEnteredKillzone(AIController character)
        {
            InitializeGameVariables();
            AddKillLocations();
            if (gv.IsInLivingBystanders(character) && !killableCharacters.Contains(character))
            {
                killableCharacters.Add(character);
            }
        }

        private void HandleCharacterLeftKillzone(AIController character)
        {
            AddKillLocations();
            InitializeGameVariables();
            killableCharacters.Remove(character);
        }

        public void HandleCharacterKilled(AIController character, bool killedByPlayer)
        {
            AddKillLocations();
            killableCharacters.Remove(character);
        }
        private void InitializeGameVariables()
        {
            if (!gv)
            {
                gv = FindObjectOfType<GameVariables>();
            }
        }

        private void AddKillLocations()
        {
            if (killLocationsAdded) return;
            foreach (KillLocation k in FindObjectsOfType<KillLocation>())
            {
                if (k.isActiveAndEnabled)
                    killLocations.Add(k);
            }
            killLocationsAdded = true;
        }
    }
}