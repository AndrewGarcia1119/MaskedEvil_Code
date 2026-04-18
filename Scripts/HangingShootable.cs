using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingShootable : MonoBehaviour, IShootable
{
    [SerializeField]
    GameObject fullHangingObject = null;
    public void OnHit(bool hitByPlayer, Transform transform)
    {
        if (!GetComponent<Rigidbody>())
        {
            fullHangingObject.AddComponent<Rigidbody>().mass = 100;
            Invoke(nameof(DestroyObject), 3f);
        }

    }

    void DestroyObject()
    {
        Destroy(fullHangingObject);
    }
}
