using System.Collections;
using System.Collections.Generic;
using GameManagement.System;
using TMPro;
using UnityEngine;

namespace UI.InGame
{
    public class GameStateUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI ammoText;

        [SerializeField]
        TextMeshProUGUI timerText;

        [SerializeField]
        TextMeshProUGUI scoreText;

        GameManager gameManager = null;
        // Update is called once per frame
        void Update()
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            if (!gameManager)
            {
                Debug.LogError("This Scene needs a gameobject of type GameManager for UI to work");
            }
            ammoText.text = gameManager.GetAmmoCount().ToString();
            timerText.text = gameManager.GetCurrentTimeLeft().ToString();
            scoreText.text = gameManager.GetCurrentScore().ToString();
        }
    }

}
