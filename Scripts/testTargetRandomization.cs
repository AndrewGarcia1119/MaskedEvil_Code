using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTargetRandomization : MonoBehaviour
{
    private IEnumerator Countdown3() {
        while(true) {
            yield return new WaitForSeconds(3);
            transform.position = new Vector3(Random.Range(-10, 10),0,Random.Range(-10, 10));
        }
    }
    void Start() {
        StartCoroutine(Countdown3());
    }
}
