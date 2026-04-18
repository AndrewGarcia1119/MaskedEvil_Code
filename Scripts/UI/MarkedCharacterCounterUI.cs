using System.Collections;
using System.Collections.Generic;
using GameManagement;
using GameManagement.System;
using TMPro;
using UnityEngine;

public class MarkedCharacterCounterUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI countText = null;

    //references
    GameVariables gv;
    GameManager gm;


    void Update()
    {
        if (!countText) return;
        if (!gv)
        {
            gv = FindObjectOfType<GameVariables>();
        }
        if (!gm)
        {
            gm = FindObjectOfType<GameManager>();
        }
        if (!gm || !gv) return;

        countText.text = $"{gv.getMarkedCount()}/{gm.GetMaxActiveMarkers()}";
    }
}
