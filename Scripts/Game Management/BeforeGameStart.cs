using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagement.System
{
    public class BeforeGameStart : MonoBehaviour
    {
        bool gameStarted = false;
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            AudioListener.pause = true;
        }

        private void Update()
        {
            if (!gameStarted)
                Cursor.lockState = CursorLockMode.None;
        }

        public void StartGame()
        {
            gameStarted = true;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            FindObjectOfType<GameManager>().OnGameStart();
            AudioListener.pause = false;
        }
    }
}