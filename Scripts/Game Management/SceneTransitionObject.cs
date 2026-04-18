using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement.System
{
    public class SceneTransitionObject : MonoBehaviour
    {
        public void ToMainMenu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        public void RestartScene()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void GoToLevel(int index)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(index);
        }
    }

}