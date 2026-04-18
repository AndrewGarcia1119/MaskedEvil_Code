using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    private GameObject[] allObjects;
    private List<GameObject> allPlayers = new List<GameObject>();

    [HideInInspector]
    public Transform activePlayerTransform;

    void Start()
    {
        //allObjects = FindObjectsOfType<GameObject>();
        GameObject mainPlayer = GameObject.FindWithTag("MainPlayer");
        allPlayers.Add(mainPlayer);
        activePlayerTransform = mainPlayer.transform;
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("PlayerTag");
        for (int i = 0; i < allObjects.Length; i++)
        {
            allPlayers.Add(allObjects[i]);
            allObjects[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Time.timeScale != 0)
        {
            int count1 = 0;
            foreach (GameObject go in allPlayers)
            {
                count1++;
                if (go.gameObject.activeSelf == true)
                {
                    go.gameObject.SetActive(false);
                    break;
                }
            }
            if (count1 == allPlayers.Count)
            {
                count1 = 0;
            }
            int count2 = 0;
            foreach (GameObject go in allPlayers)
            {
                count2++;
                if (count1 + 1 == count2)
                {
                    go.SetActive(true);
                    activePlayerTransform = go.transform;
                }
            }
        }
    }
}
