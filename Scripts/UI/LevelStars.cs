using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelStars : MonoBehaviour
{
    [SerializeField]
    Image[] stars;

    [SerializeField]
    Color completeColor, incompleteColor;

    private void OnEnable()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (PlayerPrefs.HasKey($"{i}"))
            {
                if (PlayerPrefs.GetInt($"{i}") > 0)
                {
                    stars[i].color = completeColor;
                }
                else
                {
                    stars[i].color = incompleteColor;
                }

            }
            else
            {
                stars[i].color = incompleteColor;
            }
        }
    }
}
