using System;
using System.Collections;
using System.Collections.Generic;
using GameManagement.System;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceRatingUI : MonoBehaviour
{
    [SerializeField]
    Image conditionOneCheckbox = null, conditionTwoCheckbox = null, conditionThreeCheckbox = null;

    [SerializeField]
    Color successColor, failColor;

    [SerializeField]
    float ratingDelay = 0.5f;

    [SerializeField]
    float completionDelay = 3f;


    internal event Action onShowPRComplete;

    public void ShowPerformanceRating(bool instant)
    {

        if (instant)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            conditionOneCheckbox.color = gm.GetGameConditionComplete(1) ? successColor : failColor;
            conditionTwoCheckbox.color = gm.GetGameConditionComplete(2) ? successColor : failColor;
            conditionThreeCheckbox.color = gm.GetGameConditionComplete(3) ? successColor : failColor;
            StartCoroutine(CompletionDelay());
        }
        else
        {
            StartCoroutine(PerformanceSequence());
        }
    }

    private IEnumerator PerformanceSequence()
    {
        //TODO: Implement
        GameManager gm = FindObjectOfType<GameManager>();
        yield return new WaitForSeconds(ratingDelay);
        conditionOneCheckbox.color = gm.GetGameConditionComplete(1) ? successColor : failColor;
        yield return new WaitForSeconds(ratingDelay);
        conditionTwoCheckbox.color = gm.GetGameConditionComplete(2) ? successColor : failColor;
        yield return new WaitForSeconds(ratingDelay);
        conditionThreeCheckbox.color = gm.GetGameConditionComplete(3) ? successColor : failColor;
        //This may not work, I will have to check when actually using this
        yield return CompletionDelay();

    }
    private IEnumerator CompletionDelay()
    {
        yield return new WaitForSeconds(completionDelay);
        onShowPRComplete?.Invoke();
    }

}
