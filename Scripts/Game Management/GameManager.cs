using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using Player;
using UnityEngine.Events;

namespace GameManagement.System
{
    public class GameManager : MonoBehaviour
    {
        [Header("Scoring")]
        [Tooltip("Score loss as a result of killing the wrong person")]
        [SerializeField]
        [Range(0, 1000)]
        private int wrongKillScoreLoss = 20;

        [Tooltip("Maximum score gain as a result of killing the correct person")]
        [SerializeField]
        [Range(0, 10000f)]
        private int maxCorrectKillScore = 100;

        [Tooltip("The amount the total score gets reduced after a some amount of time")]
        [SerializeField]
        private int scoreDecrement = 5;

        public string s1SaveName, s2SaveName, s3SaveName;

        [Header("Time")]
        [Tooltip("The max amount of time you have to kill the assassin (measured in seconds)")]
        [SerializeField]
        private float totalTime = 300;

        [Tooltip("The amount of time for the maximum score gain to be reduced")]
        [SerializeField]
        private float scoreDecrementTime;

        [Tooltip("The amount of time it takes to lose if not instant, if any win condition occurs during this time, the loss will be canceled")]
        [SerializeField]
        private float loseDelay = 4f;

        [Header("Player settings")]
        [SerializeField]
        [Range(1, 100)]
        private int startingAmmoCount = 5;
        [SerializeField]
        [Range(1, 20)]
        private int maxActiveMarkers = 3;

        [SerializeField]
        private AbstractGameConditions performanceConditions;

        [SerializeField]
        UnityEvent onGameEndUE;

        //references
        GameVariables gv = null;

        //states
        private int currentScore = 0;
        private float gameTime;
        private float incrementTime;
        private int correctKillScore;
        private int currentAmmoCount;
        private bool gameEnded = false;
        private bool gameStarted = false;
        private bool finalAssassination = false;
        private bool isPaused = false;

        //This event is used if the game ends. The bool should be set to true if the player won, and false if the player lost
        public static event Action<bool, Condition> onGameEnded;

        public static event Action<bool> onPauseToggled;

        private void OnEnable()
        {
            AIController.onKilled += HandleAIKilled;
            Sniper.onPlayerShoot += HandlePlayerShot;
            AIController.onMarkToggled += HandleCharacterMarkerToggled;
        }

        private void Start()
        {
            gv = FindObjectOfType<GameVariables>();
            gameTime = totalTime;
            correctKillScore = maxCorrectKillScore;
            currentAmmoCount = startingAmmoCount;
        }

        private void OnDisable()
        {
            AIController.onKilled -= HandleAIKilled;
            Sniper.onPlayerShoot -= HandlePlayerShot;
            AIController.onMarkToggled -= HandleCharacterMarkerToggled;
        }
        private void HandleAIKilled(AIController deadAI, bool killedByPlayer)
        {
            if (gameEnded) return;
            if (killedByPlayer)
            {
                currentAmmoCount--;
                if (!deadAI.IsAssassin())
                {
                    currentScore -= wrongKillScoreLoss;
                    if (deadAI == gv.designatedSurvivor)
                    {
                        LoseGame(Condition.DESIGNATED_IS_DEAD);
                    }
                    else
                    {
                        gv.livingBystanders.Remove(deadAI);
                        gv.deadCharacters.Add(deadAI);
                        CheckForNoAmmo();
                    }
                }
                else
                {
                    currentScore += correctKillScore;
                    gv.assassins.Remove(deadAI);
                    gv.deadCharacters.Add(deadAI);
                    //Win condition and score gain --> just score gain if we have a level with multiple assassins
                    if (gv.assassins.Count <= 0)
                    {
                        WinGame(Condition.ASSASSINS_KILLED);
                    }
                    else
                    {
                        CheckForNoAmmo();
                    }

                }
            }
            else
            {
                if (!deadAI.IsAssassin())
                {
                    if (deadAI == gv.designatedSurvivor)
                    {
                        LoseGame(Condition.DESIGNATED_IS_DEAD);
                    }
                    else
                    {
                        gv.livingBystanders.Remove(deadAI);
                        gv.deadCharacters.Add(deadAI);
                        CheckForNoAmmo();
                    }

                }
                else
                {
                    currentScore += correctKillScore;
                    gv.assassins.Remove(deadAI);
                    gv.deadCharacters.Add(deadAI);
                    //Win condition and score gain --> just score gain if we have a level with multiple assassins
                    if (gv.assassins.Count <= 0)
                    {
                        WinGame(Condition.ASSASSINS_KILLED);
                    }
                }
            }
        }

        //reduces total ammo each time player shoots, if a shootable is hit
        private void HandlePlayerShot(bool hitAI)
        {
            if (gameEnded) return;
            if (hitAI) return;
            currentAmmoCount--;
            CheckForNoAmmo();
        }

        private void HandleCharacterMarkerToggled(AIController character, bool marked)
        {
            if (marked)
            {
                gv.markedCharacters.Add(character);
                if (gv.markedCharacters.Count > maxActiveMarkers)
                {
                    gv.markedCharacters[0].ToggleMark(false);
                }
            }
            else
            {
                gv.markedCharacters.Remove(character);
            }
        }

        private void WinGame(Condition condition)
        {
            gameEnded = true;
            onGameEnded?.Invoke(true, condition);
            onGameEndUE?.Invoke();
            SaveConditionState(s1SaveName, 1);
            SaveConditionState(s2SaveName, 2);
            SaveConditionState(s3SaveName, 3);
            PlayerPrefs.Save();

        }

        public void SaveConditionState(string conditionName, int val)
        {
            if (conditionName != null && conditionName != "")
            {
                if (!PlayerPrefs.HasKey(conditionName) || PlayerPrefs.GetInt(conditionName) == 0)
                {
                    PlayerPrefs.SetInt(conditionName, GetGameConditionComplete(val) ? 1 : 0);
                }
            }
        }

        private void LoseGame(Condition condition)
        {
            gameEnded = true;
            onGameEnded?.Invoke(false, condition);
            onGameEndUE?.Invoke();
        }


        private void DelayedLoseGame(Condition condition)
        {
            StartCoroutine(LoseGameSequence(condition));
        }

        private IEnumerator LoseGameSequence(Condition condition)
        {
            yield return new WaitForSeconds(loseDelay);
            if (!gameEnded)
            {
                gameEnded = true;
                onGameEnded?.Invoke(false, condition);
                onGameEndUE?.Invoke();
            }
        }

        private void CheckForNoAmmo()
        {
            if (currentAmmoCount <= 0)
            {
                foreach (Sniper sniper in gv.snipers)
                {
                    sniper.hasAmmo = false;
                }
                DelayedLoseGame(Condition.NO_AMMO);
            }
        }

        private void Update()
        {
            if (!gameStarted) return;
            if (gameEnded) return;
            incrementTime += Time.deltaTime;
            gameTime -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    PauseGame(true);
                }
                else
                {
                    PauseGame(false);
                }
            }
            if (isPaused) return;
            if (incrementTime >= scoreDecrementTime)
            {
                incrementTime = 0;
                correctKillScore -= scoreDecrement;
            }
            if (!finalAssassination && gameTime <= 0)
            {
                finalAssassination = true;
                foreach (AIController assassin in gv.assassins)
                {
                    assassin.OverrideAssassinKillDS();
                }
                //Set Condition to force Assassin to go straight for designated survivor
            }

        }

        internal void OnGameStart()
        {
            gameStarted = true;
        }

        public void PauseGame(bool pause)
        {
            onPauseToggled?.Invoke(pause);
            Time.timeScale = pause ? 0f : 1f;
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
            AudioListener.pause = pause;
        }

        public bool GetGameConditionComplete(int conditionIndex)
        {
            if (performanceConditions == null) return false;

            if (conditionIndex == 1)
            {
                return performanceConditions.FirstConditionCompleted();
            }
            else if (conditionIndex == 2)
            {
                return performanceConditions.SecondConditonCompleted();
            }
            else if (conditionIndex == 3)
            {
                return performanceConditions.ThirdConditionCompleted();
            }
            else
            {
                return false;
            }
        }

        public int GetCurrentScore()
        {
            return currentScore;
        }

        public float GetTotalTime()
        {
            return totalTime;
        }
        public float GetCurrentTimeLeft()
        {
            int temp = (int)(gameTime * 100);
            return Mathf.Max((float)temp / 100, 0);
        }

        public int GetAmmoCount()
        {
            return currentAmmoCount;
        }

        public int GetStartingAmmoCount()
        {
            return startingAmmoCount;
        }

        public int GetMaxActiveMarkers()
        {
            return maxActiveMarkers; ;
        }

        public enum Condition
        {
            NO_AMMO, DESIGNATED_IS_DEAD, ASSASSINS_KILLED
        }
    }
}