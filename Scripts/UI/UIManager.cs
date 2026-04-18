using System;
using System.Collections;
using System.Collections.Generic;
using GameManagement.System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameStateCanvas;

    [SerializeField]
    private GameObject winLoseCanvas;

    [SerializeField]
    private TextMeshProUGUI winLoseText;

    [SerializeField]
    private TextMeshProUGUI conditionText;

    [SerializeField]
    private PerformanceRatingUI pfUI = null;

    [SerializeField]
    private GameObject[] delayedDisplayObjects = null;

    [SerializeField]
    private GameObject hintNObjectiveCanvas = null;

    [SerializeField]
    private GameObject toggleHintsText;

    [SerializeField]
    private GameObject gameStartCanvas = null;

    [SerializeField]
    private GameObject toggleObjectivesCanvas = null;

    [SerializeField]
    float delayPFResultsTimer = 2f;

    [SerializeField]
    GameObject pauseMenuObjects = null;


    GameManager gm;

    private bool gameStarted = false;
    private void OnEnable()
    {
        GameManager.onGameEnded += HandleEnding;
        pfUI.onShowPRComplete += ShowFinalWinScreen;
    }


    private void Start()
    {
        winLoseCanvas.gameObject.SetActive(false);
        if (delayedDisplayObjects != null)
        {
            foreach (GameObject g in delayedDisplayObjects)
            {
                g.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!gameStarted) return;
        if (!hintNObjectiveCanvas) return;
        if (!gm)
        {
            gm = FindObjectOfType<GameManager>();
        }
        if (Time.timeScale != 0)
        {
            if (pauseMenuObjects)
            {
                pauseMenuObjects.SetActive(false);
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                hintNObjectiveCanvas.SetActive(!hintNObjectiveCanvas.activeSelf);
            }
        }
        else
        {
            hintNObjectiveCanvas.SetActive(false);
            if (pauseMenuObjects)
            {
                pauseMenuObjects.SetActive(true);
            }
        }

    }

    public void OnGameStartUI()
    {
        Destroy(gameStartCanvas);
        gameStarted = true;
        toggleHintsText.SetActive(true);
        hintNObjectiveCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        GameManager.onGameEnded -= HandleEnding;
    }

    private void ShowFinalWinScreen()
    {
        pfUI.gameObject.SetActive(false);
        foreach (GameObject g in delayedDisplayObjects)
        {
            g.SetActive(true);
        }
    }

    private IEnumerator WinScreenSequence()
    {
        yield return new WaitForSeconds(delayPFResultsTimer);
        gm.SaveConditionState(gm.s1SaveName, 1);
        gm.SaveConditionState(gm.s2SaveName, 2);
        gm.SaveConditionState(gm.s3SaveName, 3);
        PlayerPrefs.Save();
        pfUI.gameObject.SetActive(true);
        pfUI.ShowPerformanceRating(false);
    }

    private void HandleEnding(bool hasWon, GameManager.Condition condition)
    {
        /*
        If the player wins, it should not immediately show the buttons, it should first show the performance rating, then in shows the buttons

        It should enable the Performance Rating UI, then it should call a method on it to show the results.
        */
        if (hasWon)
        {
            winLoseText.text = "You Win!";
            conditionText.text = "There are no more assassins!";
            winLoseCanvas.gameObject.SetActive(true);
            StartCoroutine(WinScreenSequence());
        }
        else
        {
            winLoseText.text = "You Lose!";
            if (condition == GameManager.Condition.NO_AMMO)
            {
                conditionText.text = "You ran out of ammo!";
            }
            else if (condition == GameManager.Condition.DESIGNATED_IS_DEAD)
            {
                conditionText.text = "The host is dead!";

            }
            foreach (GameObject g in delayedDisplayObjects)
            {
                g.SetActive(true);
            }
            winLoseCanvas.gameObject.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        if (hintNObjectiveCanvas)
        {
            Destroy(hintNObjectiveCanvas);
            Destroy(toggleObjectivesCanvas);
            hintNObjectiveCanvas = null;
        }
    }
}
